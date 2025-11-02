<template>
    <v-app>
        <v-main>
            <v-container class="py-10">
                <div class="text-center mb-6">
                    <h1 class="text-h4 font-weight-bold text-grey-darken-3">Floorplan Designer</h1>
                </div>
                <v-card 
                    class="mx-auto pa-4 mb-6"
                    max-width="800"
                    elevation="4"
                >
                    <div class="d-flex align-center flex-wrap ga-3">

                        <!-- Clear Button -->
                        <v-btn
                            color="pink"
                            @click="clearFloorplan"
                        >
                            <v-icon start icon="mdi-delete"></v-icon> Clear All
                        </v-btn>

                    </div>
                </v-card>

                <!-- Canvas Container -->
                <div class="d-flex justify-center">
                    <canvas
                        ref="canvasRef"
                        id="floorplanCanvas"
                        @mousedown="handleMouseDown"
                        @mousemove="handleMouseMove"
                        @mouseup="handleMouseUp"
                        @touchstart.prevent="handleMouseDown"
                        @touchmove.prevent="handleMouseMove"
                        @touchend="handleMouseUp"
                        style="touch-action: none; cursor: crosshair;"
                    />
                </div>

                <!-- Status Message -->
                <v-alert
                    :text="statusMessage"
                    :color="statusColorMap[statusClass]"
                    :icon="statusIconMap[statusClass]"
                    :variant="statusClass === 'text-red-500' ? 'tonal' : 'elevated'"
                    class="mt-4 mx-auto"
                    max-width="800"
                />

            </v-container>
        </v-main>
    </v-app>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.6.1/firebase-app.js";
import { getAuth, signInAnonymously, signInWithCustomToken } from "https://www.gstatic.com/firebasejs/11.6.1/firebase-auth.js";
import { getFirestore, doc, setDoc, onSnapshot } from "https://www.gstatic.com/firebasejs/11.6.1/firebase-firestore.js";

// --- Global Constants from Environment ---
const appId = typeof __app_id !== 'undefined' ? __app_id : 'default-app-id';
const firebaseConfig = typeof __firebase_config !== 'undefined' ? JSON.parse(__firebase_config) : {};
const initialAuthToken = typeof __initial_auth_token !== 'undefined' ? __initial_auth_token : null;

// --- Configuration ---
const GRID_SIZE = 20;
const WALL_THICKNESS = 5;
const GRID_COLOR = '#e5e7eb';
const WALL_COLOR = '#374151';
const TEMP_WALL_COLOR = '#818cf8';

// Vuetify mapping for status
const statusColorMap = {
    'text-gray-600': 'grey',
    'text-indigo-600': 'indigo',
    'text-yellow-600': 'warning',
    'text-green-600': 'success',
    'text-blue-600': 'info',
    'text-red-500': 'error',
};
const statusIconMap = {
    'text-gray-600': 'mdi-information',
    'text-indigo-600': 'mdi-account-check',
    'text-yellow-600': 'mdi-content-save-edit',
    'text-green-600': 'mdi-check-circle',
    'text-blue-600': 'mdi-download',
    'text-red-500': 'mdi-alert-circle',
};

// Globals for Firebase
let db;
let auth;

// Vue Reactive States
const walls = ref([]);
const isDrawing = ref(false);
const startPoint = ref({ x: 0, y: 0 });
const tempEndPoint = ref({ x: 0, y: 0 });
const currentTool = ref('drawWall');
const statusMessage = ref('Initializing application...');
const statusClass = ref('text-gray-600'); 
const userId = ref(null);
const isDbReady = ref(false);
const canvasRef = ref(null);

// Utilities

/** Updates the status message displayed below the canvas. */
const updateStatus = (message, colorClass = 'text-gray-600') => {
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
        ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, canvas.height); ctx.stroke();
    }
    for (let y = 0; y <= canvas.height; y += GRID_SIZE) {
        ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(canvas.width, y); ctx.stroke();
    }
};

const drawWall = (ctx, x1, y1, x2, y2, color) => {
    ctx.strokeStyle = color;
    ctx.lineWidth = WALL_THICKNESS;
    ctx.lineCap = 'round';
    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
};

const draw = () => {
    const canvas = canvasRef.value;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    drawGrid(ctx, canvas);

    walls.value.forEach(wall => {
        drawWall(ctx, wall.startX, wall.startY, wall.endX, wall.endY, WALL_COLOR);
    });

    if (isDrawing.value) {
        drawWall(ctx, startPoint.value.x, startPoint.value.y, tempEndPoint.value.x, tempEndPoint.value.y, TEMP_WALL_COLOR);
    }
};


// Firestore Actions

const saveWallsToDb = async (newWalls) => {
    if (!isDbReady.value || !userId.value || !db) return;

    const docRef = getWallDocRef();
    updateStatus('Saving floorplan...', 'text-yellow-600');
    try {
        await setDoc(docRef, { 
            walls: newWalls,
            updatedAt: Date.now()
        });
        updateStatus('Floorplan saved successfully.', 'text-green-600');
    } catch (error) {
        console.error("Error saving document: ", error);
        updateStatus('Error saving plan. Check console.', 'text-red-500');
    }
};

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
        y: clientY - rect.top
    };
};

const handleMouseDown = (event) => {
    if (currentTool.value !== 'drawWall') return;

    const { x, y } = getCanvasCoords(event);

    isDrawing.value = true;
    const snappedStart = { x: snapToGrid(x), y: snapToGrid(y) };
    startPoint.value = snappedStart;
    tempEndPoint.value = snappedStart;
    draw();
};

const handleMouseMove = (event) => {
    if (!isDrawing.value || currentTool.value !== 'drawWall') return;

    const { x, y } = getCanvasCoords(event);
    let snappedX = snapToGrid(x);
    let snappedY = snapToGrid(y);

    const dx = Math.abs(snappedX - startPoint.value.x);
    const dy = Math.abs(snappedY - startPoint.value.y);
    const tolerance = GRID_SIZE / 2;
    let newTempEndPoint;

    if (dx > dy && dx > tolerance) {
        newTempEndPoint = { x: snappedX, y: startPoint.value.y };
    } else if (dy > dx && dy > tolerance) {
        newTempEndPoint = { x: startPoint.value.x, y: snappedY };
    } else {
        newTempEndPoint = { x: snappedX, y: snappedY };
    }

    tempEndPoint.value = newTempEndPoint;
    draw();
};

const handleMouseUp = () => {
    if (!isDrawing.value || currentTool.value !== 'drawWall') return;

    isDrawing.value = false;

    if (startPoint.value.x !== tempEndPoint.value.x || startPoint.value.y !== tempEndPoint.value.y) {
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
    draw();
};

const clearFloorplan = () => {
    walls.value = [];
    saveWallsToDb([]);
    updateStatus('Floorplan cleared. Saving empty plan...', 'text-gray-600');
};

const setTool = (tool) => {
    currentTool.value = tool;
    updateStatus('Drawing tool selected. Click and drag to create walls.', 'text-gray-600');
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

    onSnapshot(docRef, (docSnap) => {
        if (docSnap.exists() && docSnap.data().walls) {
            const loadedWalls = docSnap.data().walls;
            
            // Only update the local state if the incoming data is actually different
            if (JSON.stringify(loadedWalls) !== JSON.stringify(walls.value)) {
                walls.value = loadedWalls;
                updateStatus('Floorplan loaded from database.', 'text-blue-600');
            }
        } else {
            updateStatus('Starting new floorplan (no existing plan found in DB).', 'text-gray-600');
        }
    }, (error) => {
        console.error("Snapshot Listener Error:", error);
        updateStatus('Failed to listen for real-time updates.', 'text-red-500');
    });
};

onMounted(async () => {
    // 1. Canvas Initialization and Resize Handling
    const handleResize = () => {
        const canvas = canvasRef.value;
        if (!canvas) return;
        const container = canvas.parentElement;
        // Max canvas width is 800px, but it will scale down responsively
        const size = Math.min(container.clientWidth, 800); 
        
        canvas.width = snapToGrid(size); 
        canvas.height = snapToGrid(size);
        
        draw();
    };

    handleResize();
    window.addEventListener('resize', handleResize);
    
    // 2. Firebase Initialization and Authentication
    if (Object.keys(firebaseConfig).length === 0) {
        updateStatus('Database is unavailable. Data will not be saved. (Missing config)', 'text-red-500');
        return;
    }

    try {
        const app = initializeApp(firebaseConfig);
        auth = getAuth(app);
        db = getFirestore(app);
        isDbReady.value = true;

        updateStatus('Authenticating...');
        if (initialAuthToken) {
            await signInWithCustomToken(auth, initialAuthToken);
        } else {
            await signInAnonymously(auth);
        }

        const currentUserId = auth.currentUser?.uid || crypto.randomUUID();
        userId.value = currentUserId;
        updateStatus(`User ID: ${currentUserId}. Loading plan...`);
    } catch (error) {
        console.error("Firebase Initialization Error:", error);
        updateStatus('Authentication failed. Data saving disabled.', 'text-red-500');
        isDbReady.value = false;
    }
});
</script>

<style scoped>
/* Scoped styles for non-Vuetify element */
#floorplanCanvas {
    border: 2px solid #cbd5e1;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    background-color: #ffffff;
}
</style>
