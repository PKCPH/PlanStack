<template>
  <v-app>
    <v-main>
      <v-container class="py-10">
        <div class="text-center mb-6">
          <h1 class="text-h4 font-weight-bold text-grey-darken-3">
            Floorplans
          </h1>
        </div>

        <div class="d-flex justify-center" v-if="showCanvas">
          <canvas
            ref="canvasRef"
            id="floorplanCanvas"
            @mousedown="handleMouseDown"
            @mousemove="handleMouseMove"
            @mouseup="handleMouseUp"
            @mouseleave="handleMouseLeave"
            @touchstart.prevent="handleMouseDown"
            @touchmove.prevent="handleMouseMove"
            @touchend="handleMouseUp"
            style="touch-action: none; cursor: crosshair"
          />
        </div>

        <v-alert
          v-if="showCanvas"
          :text="statusMessage"
          :color="statusColorMap[statusClass]"
          :icon="statusIconMap[statusClass]"
          :variant="statusClass === 'text-red-500' ? 'tonal' : 'elevated'"
          class="mt-4 mx-auto"
          max-width="800"
        />

        <Teleport to="#right-tools-container">
          <!-- Blueprint info and saving -->
          <v-list-subheader>Blueprint Details</v-list-subheader>

          <v-text-field
            v-model="blueprintName"
            label="Name"
            type="text"
            density="compact"
            class="mb-2"
            :rules="[(v) => !!v || 'Name is required']"
          ></v-text-field>
          <v-textarea
            v-model="blueprintDescription"
            label="Description (Optional)"
            rows="2"
            density="compact"
            class="mb-2"
          ></v-textarea>

          <v-row>
            <v-col cols="6">
              <v-text-field
                v-model.number="maxOccupancy"
                label="Max Occupancy"
                type="number"
                min="0"
                density="compact"
              ></v-text-field>
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="floor"
                label="Floor"
                type="number"
                min="0"
                density="compact"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-btn
            color="success"
            block
            @click="handleSave"
            :loading="isSaving"
            :disabled="!blueprintName || isSaving"
            class="mt-1"
          >
            <v-icon start>mdi-content-save</v-icon>
            Save Blueprint to API
          </v-btn>

          <v-alert
            v-if="saveMessage"
            :type="saveMessage.includes('Error') ? 'error' : 'success'"
            density="compact"
            class="mt-3"
            closable
          >
            {{ saveMessage }}
          </v-alert>

          <v-divider class="my-6"></v-divider>

          <!-- setting up canvas -->
          <v-list-subheader>Canvas Setup</v-list-subheader>
          <p class="text-caption mb-4">
            Set the initial grid dimensions for the canvas.
          </p>

          <v-row>
            <v-col cols="6">
              <v-text-field
                v-model.number="canvasWidthCells"
                label="Width (Cells)"
                type="number"
                min="10"
                max="100"
                density="compact"
              ></v-text-field>
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="canvasHeightCells"
                label="Height (Cells)"
                type="number"
                min="10"
                max="100"
                density="compact"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-btn color="primary" block @click="createCanvas" class="mb-6">
            Create/Resize Canvas
          </v-btn>

          <v-divider class="my-6"></v-divider>

          <!-- drawing -->
          <v-list-subheader>Drawing Tools</v-list-subheader>
          <div class="d-flex align-center flex-wrap ga-3">
            <v-btn color="pink" @click="clearFloorplan" size="small">
              <v-icon start icon="mdi-delete"></v-icon> Clear All
            </v-btn>

            <v-btn
              :color="
                currentTool === 'eraseWall' ? 'red-lighten-2' : 'grey-lighten-1'
              "
              @click="setTool('eraseWall')"
              variant="flat"
              size="small"
            >
              <v-icon start icon="mdi-eraser"></v-icon> Erase Wall
            </v-btn>

            <v-btn
              :color="currentTool === 'drawWall' ? 'indigo' : 'grey-lighten-1'"
              @click="setTool('drawWall')"
              variant="flat"
              size="small"
            >
              <v-icon start icon="mdi-pencil"></v-icon> Draw Wall
            </v-btn>
          </div>
        </Teleport>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from "vue";
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.6.1/firebase-app.js";
import {
  getAuth,
  signInAnonymously,
  signInWithCustomToken,
} from "https://www.gstatic.com/firebasejs/11.6.1/firebase-auth.js";
import {
  getFirestore,
  doc,
  setDoc,
  onSnapshot,
} from "https://www.gstatic.com/firebasejs/11.6.1/firebase-firestore.js";
import { useTheme } from "vuetify";

// --- Global Constants from Environment ---
const theme = useTheme();
const appId = typeof __app_id !== "undefined" ? __app_id : "default-app-id";
const firebaseConfig =
  typeof __firebase_config !== "undefined" ? JSON.parse(__firebase_config) : {};
const initialAuthToken =
  typeof __initial_auth_token !== "undefined" ? __initial_auth_token : null;

// --- Configuration ---
const GRID_SIZE = 20;
const WALL_THICKNESS = 5;
const GRID_COLOR = "#e5e7eb";
const WALL_COLOR = theme.global.current.value.colors.primary;
const TEMP_WALL_COLOR = "#818cf8";

// Vuetify mapping for status
const statusColorMap = {
  "text-gray-600": "grey",
  "text-indigo-600": "indigo",
  "text-yellow-600": "warning",
  "text-green-600": "success",
  "text-blue-600": "info",
  "text-red-500": "error",
};
const statusIconMap = {
  "text-gray-600": "mdi-information",
  "text-indigo-600": "mdi-account-check",
  "text-yellow-600": "mdi-content-save-edit",
  "text-green-600": "mdi-check-circle",
  "text-blue-600": "mdi-download",
  "text-red-500": "mdi-alert-circle",
};

// Globals for Firebase
let db;
let auth;

// Vue Reactive States
const walls = ref([]);
const isDrawing = ref(false);
const startPoint = ref({ x: 0, y: 0 });
const tempEndPoint = ref({ x: 0, y: 0 });
const currentTool = ref("drawWall");
const statusMessage = ref("Initializing application...");
const statusClass = ref("text-gray-600");
const userId = ref(null);
const isDbReady = ref(false);
const canvasRef = ref(null);
const showCanvas = ref(false);
const canvasWidthCells = ref(40);
const canvasHeightCells = ref(40);
const finalCanvasSize = ref({ width: 0, height: 0 });
const ERASE_TOLERANCE = 10;
const hoveredWall = ref(null);
const ERASE_HIGHLIGHT_COLOR = "#ef4444";

// Blueprint form fields state
const blueprintName = ref("");
const blueprintDescription = ref("");
const maxOccupancy = ref(0);
const floor = ref(1);

// API Logic Integration from useBlueprintsApi.js
// Using a CORS proxy to bypass cross-origin restrictions in the browser
const CORS_PROXY_URL = "https://corsproxy.io/?";
const BLUEPRINTS_API_URL = "http://planstack.dk/api/api/blueprints";

const isSaving = ref(false);
const saveMessage = ref("");
const lastSavedBlueprint = ref(null);

// request to create blueprint using native fetch (not using axios for now)

const saveBlueprintToAPI = async (blueprintData) => {
  if (!blueprintData.name) {
    saveMessage.value = "Error: Blueprint Name is required.";
    return;
  }

  isSaving.value = true;
  saveMessage.value = "Saving blueprint...";
  lastSavedBlueprint.value = null;

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(BLUEPRINTS_API_URL)}`;

    const response = await fetch(proxiedUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(blueprintData),
    });

    if (!response.ok) {
      let errorDetail = response.statusText;
      try {
        const errorData = await response.json();
        errorDetail = errorData.message || errorDetail;
      } catch (e) {}
      throw new Error(
        `API returned error: Status ${response.status}. Detail: ${errorDetail}`
      );
    }

    const data = await response.json();
    console.log("API Response:", data);

    lastSavedBlueprint.value = data;
    saveMessage.value = `Blueprint saved successfully!`;
    return data;
  } catch (error) {
    console.error("Error saving blueprint:", error);
    saveMessage.value = `Error saving blueprint: ${error.message || "An unknown error occurred"}.`;
    throw error;
  } finally {
    isSaving.value = false;
  }
};

// Utilities

/** Updates the status message displayed below the canvas. */
const updateStatus = (message, colorClass = "text-gray-600") => {
  statusMessage.value = message;
  statusClass.value = colorClass;
};

/** Gets the Firestore document reference for the current user's plan. */
const getWallDocRef = () => {
  if (!db || !userId.value) return null;
  const path = `/artifacts/${appId}/users/${userId.value}/floorplans/current_plan`;
  return doc(db, path);
};

// Ensures coordinates snap to the nearest grid intersection.
const snapToGrid = (coord) => Math.round(coord / GRID_SIZE) * GRID_SIZE;

// Drawing Helpers
const drawGrid = (ctx, canvas) => {
  ctx.strokeStyle = GRID_COLOR;
  ctx.lineWidth = 1;
  for (let x = 0; x <= canvas.width; x += GRID_SIZE) {
    ctx.beginPath();
    ctx.moveTo(x, 0);
    ctx.lineTo(x, canvas.height);
    ctx.stroke();
  }
  for (let y = 0; y <= canvas.height; y += GRID_SIZE) {
    ctx.beginPath();
    ctx.moveTo(0, y);
    ctx.lineTo(canvas.width, y);
    ctx.stroke();
  }
};

const drawWall = (ctx, x1, y1, x2, y2, color) => {
  ctx.strokeStyle = color;
  ctx.lineWidth = WALL_THICKNESS;
  ctx.lineCap = "round";
  ctx.beginPath();
  ctx.moveTo(x1, y1);
  ctx.lineTo(x2, y2);
  ctx.stroke();
};

const draw = () => {
  const canvas = canvasRef.value;
  if (!canvas) return;
  const ctx = canvas.getContext("2d");

  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawGrid(ctx, canvas);

  // Draw all walls normally
  walls.value.forEach((wall) => {
    if (hoveredWall.value && wall === hoveredWall.value) {
      return;
    }
    drawWall(ctx, wall.startX, wall.startY, wall.endX, wall.endY, WALL_COLOR);
  });

  if (hoveredWall.value && currentTool.value === "eraseWall") {
    drawWall(
      ctx,
      hoveredWall.value.startX,
      hoveredWall.value.startY,
      hoveredWall.value.endX,
      hoveredWall.value.endY,
      ERASE_HIGHLIGHT_COLOR
    );
  }

  if (isDrawing.value && currentTool.value === "drawWall") {
    drawWall(
      ctx,
      startPoint.value.x,
      startPoint.value.y,
      tempEndPoint.value.x,
      tempEndPoint.value.y,
      TEMP_WALL_COLOR
    );
  }
};

const isPointNearWall = (px, py, wall) => {
  const { startX, startY, endX, endY } = wall;

  const l2 = (endX - startX) ** 2 + (endY - startY) ** 2;
  if (l2 === 0) return false; // Not a segment

  let t =
    ((px - startX) * (endX - startX) + (py - startY) * (endY - startY)) / l2;
  t = Math.max(0, Math.min(1, t));
  const closestX = startX + t * (endX - startX);
  const closestY = startY + t * (endY - startY);
  const distance = Math.sqrt((px - closestX) ** 2 + (py - closestY) ** 2);

  return distance <= WALL_THICKNESS + ERASE_TOLERANCE;
};

// const saveWallsToDb = async (newWalls) => {
//   if (!isDbReady.value || !userId.value || !db) return;

//   const docRef = getWallDocRef();
//   updateStatus("Syncing floorplan data...", "text-yellow-600");
//   try {
//     await setDoc(docRef, {
//       walls: newWalls,
//       updatedAt: Date.now(),
//     });
//     updateStatus("Drawing data synced.", "text-green-600");
//   } catch (error) {
//     console.error("Error saving document: ", error);
//     updateStatus("Error syncing drawing data. Check console.", "text-red-500");
//   }
// };

// Event Handlers

const getCanvasCoords = (event) => {
  const canvas = canvasRef.value;
  let clientX = event.clientX;
  let clientY = event.clientY;

  if (event.touches) {
    clientX = event.touches[0].clientX;
    clientY = event.touches[0].clientY;
  }

  const rect = canvas.getBoundingClientRect();
  return {
    x: clientX - rect.left,
    y: clientY - rect.top,
  };
};

const handleMouseDown = (event) => {
  const { x, y } = getCanvasCoords(event);
  const snappedPoint = { x: snapToGrid(x), y: snapToGrid(y) };

  if (currentTool.value === "drawWall") {
    isDrawing.value = true;
    startPoint.value = snappedPoint;
    tempEndPoint.value = snappedPoint;
    draw();
  } else if (currentTool.value === "eraseWall") {
    // Use non-snapped coords for accurate erase hit test
    startPoint.value = { x, y };
    isDrawing.value = true;
  }
};

const handleMouseUp = () => {
  if (!isDrawing.value) return;

  isDrawing.value = false;
  hoveredWall.value = null; // Clear hover state on mouse up

  if (currentTool.value === "drawWall") {
    if (
      startPoint.value.x !== tempEndPoint.value.x ||
      startPoint.value.y !== tempEndPoint.value.y
    ) {
      const newWall = {
        startX: startPoint.value.x,
        startY: startPoint.value.y,
        endX: tempEndPoint.value.x,
        endY: tempEndPoint.value.y,
      };

      const updatedWalls = [...walls.value, newWall];
      walls.value = updatedWalls;
      saveWallsToDb(updatedWalls);
    }
  } else if (currentTool.value === "eraseWall") {
    // Erase logic: check if the click point is near any wall
    const clickX = startPoint.value.x;
    const clickY = startPoint.value.y;

    const wallIndexToErase = walls.value.findIndex((wall) =>
      isPointNearWall(clickX, clickY, wall)
    );

    if (wallIndexToErase !== -1) {
      // Remove the wall
      const updatedWalls = walls.value.filter(
        (_, index) => index !== wallIndexToErase
      );
      walls.value = updatedWalls;
      saveWallsToDb(updatedWalls);
      updateStatus("Wall erased. Syncing plan...", "text-yellow-600");
    } else {
      updateStatus("No wall found to erase at that location.", "text-gray-600");
    }
  }
  draw();
};

const handleMouseMove = (event) => {
  const { x, y } = getCanvasCoords(event);

  if (!isDrawing.value) {
    // Handle hover highlighting in erase mode when not drawing
    if (currentTool.value === "eraseWall") {
      const wallUnderCursor = walls.value.find((wall) =>
        isPointNearWall(x, y, wall)
      );
      if (wallUnderCursor !== hoveredWall.value) {
        hoveredWall.value = wallUnderCursor;
        draw();
      }
    }
    return;
  }

  if (currentTool.value === "drawWall") {
    let snappedX = snapToGrid(x);
    let snappedY = snapToGrid(y);

    const dx = Math.abs(snappedX - startPoint.value.x);
    const dy = Math.abs(snappedY - startPoint.value.y);
    const tolerance = GRID_SIZE / 2;
    let newTempEndPoint;

    // Constrain to orthogonal drawing
    if (dx > dy && dx > tolerance) {
      newTempEndPoint = { x: snappedX, y: startPoint.value.y };
    } else if (dy > dx && dy > tolerance) {
      newTempEndPoint = { x: startPoint.value.x, y: snappedY };
    } else {
      newTempEndPoint = { x: snappedX, y: snappedY };
    }

    tempEndPoint.value = newTempEndPoint;
    draw();
  }
};

const handleMouseLeave = () => {
  if (currentTool.value === "eraseWall") {
    hoveredWall.value = null;
    draw();
  }
};

const clearFloorplan = () => {
  walls.value = [];
  saveWallsToDb([]);
  updateStatus("Floorplan cleared. Syncing empty plan...", "text-gray-600");
};

const setTool = (tool) => {
  currentTool.value = tool;
  if (tool === "drawWall") {
    updateStatus(
      "Drawing tool selected. Click and drag to create walls.",
      "text-indigo-600"
    );
  } else if (tool === "eraseWall") {
    updateStatus(
      "Eraser tool selected. Click on a wall to remove it.",
      "text-red-500"
    );
  } else {
    updateStatus(`Tool ${tool} selected.`, "text-gray-600");
  }
};

const setupCanvasAndListeners = () => {
  const canvas = canvasRef.value;
  if (!canvas) return;

  const widthPixels = finalCanvasSize.value.width;
  const heightPixels = finalCanvasSize.value.height;

  canvas.width = widthPixels;
  canvas.height = heightPixels;

  draw();

  // Start Firebase auth if not already started
  if (!userId.value) {
    startFirebaseInitialization();
  }
};

const createCanvas = () => {
  const widthCells = Number(canvasWidthCells.value);
  const heightCells = Number(canvasHeightCells.value);

  if (
    isNaN(widthCells) ||
    isNaN(heightCells) ||
    widthCells < 10 ||
    heightCells < 10
  ) {
    updateStatus(
      "Please enter valid canvas dimensions (min 10x10).",
      "text-red-500"
    );
    return;
  }

  finalCanvasSize.value = {
    width: widthCells * GRID_SIZE,
    height: heightCells * GRID_SIZE,
  };

  showCanvas.value = true;

  nextTick(() => {
    setupCanvasAndListeners();
  });
};

/**
 * Handles the main save operation, structuring the data and calling the API.
 */
const handleSave = async () => {
  if (!blueprintName.value) {
    saveMessage.value = "Error: Blueprint name is required to save.";
    return;
  }

  const drawingDataPayload = {
    walls: walls.value,
    gridSize: GRID_SIZE,
    canvasWidth: finalCanvasSize.value.width,
    canvasHeight: finalCanvasSize.value.height,
  };

  // main payload
  const apiPayload = {
    name: blueprintName.value,
    description: blueprintDescription.value,
    max_occupancy: maxOccupancy.value,
    floor: floor.value,

    // Canvas size
    height: canvasHeightCells.value,
    width: canvasWidthCells.value,
  };

  await saveBlueprintToAPI(apiPayload);
};

// Firebase Init (this iscalled after canvas is creation)
const startFirebaseInitialization = async () => {
  if (Object.keys(firebaseConfig).length === 0) {
    updateStatus(
      "Database is unavailable. Data will not be synced. (Missing config)",
      "text-red-500"
    );
    return;
  }

  try {
    const app = initializeApp(firebaseConfig);
    auth = getAuth(app);
    db = getFirestore(app);
    isDbReady.value = true;

    updateStatus("Authenticating...");
    if (initialAuthToken) {
      await signInWithCustomToken(auth, initialAuthToken);
    } else {
      await signInAnonymously(auth);
    }

    const currentUserId = auth.currentUser?.uid || crypto.randomUUID();
    userId.value = currentUserId;
    updateStatus(`User ID: ${currentUserId}. Loading and syncing plan...`);
  } catch (error) {
    console.error("Firebase Initialization Error:", error);
    updateStatus("Authentication failed. Syncing disabled.", "text-red-500");
    isDbReady.value = false;
  }
};

// Vue Lifecycle an Watchers

// Watch walls state to trigger redraw when data is loaded from Firebase
watch(walls, draw, { deep: true });

// Watch user ID to start the Firebase listener once authenticated
watch(userId, (newUserId) => {
  if (isDbReady.value && newUserId) {
    listenForPlanUpdates();
  }
});

/** Listens for real-time changes to the plan from Firestore. */
const listenForPlanUpdates = () => {
  const docRef = getWallDocRef();
  if (!docRef) return;

  onSnapshot(
    docRef,
    (docSnap) => {
      if (docSnap.exists() && docSnap.data().walls) {
        const loadedWalls = docSnap.data().walls;

        // Only update the local state if the incoming data is actually different
        if (JSON.stringify(loadedWalls) !== JSON.stringify(walls.value)) {
          walls.value = loadedWalls;
          updateStatus("Drawing data loaded from database.", "text-blue-600");
        }
      } else {
        updateStatus(
          "Starting new floorplan (no existing drawing data found).",
          "text-gray-600"
        );
      }
    },
    (error) => {
      console.error("Snapshot Listener Error:", error);
      updateStatus("Failed to listen for real-time updates.", "text-red-500");
    }
  );
};
</script>

<style scoped>
/* Scoped styles for non-Vuetify element */
#floorplanCanvas {
  border: 2px solid #cbd5e1;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  background-color: #ffffff;
}
</style>
