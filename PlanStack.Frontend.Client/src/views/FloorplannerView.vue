<template>
  <v-container class="py-10" max-width="800">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">
        Floorplans for {{ projectName }}
      </h1>
    </div>

    <div
      class="d-flex justify-center align-center canvas-viewport"
      v-if="showCanvas"
      ref="viewportRef"
    >
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
        :style="canvasStyle"
      />
    </div>

    <Teleport to="#right-tools-container">
      <div class="d-flex justify-space-between align-center mb-2">
        <v-list-subheader class="pa-0">Blueprints</v-list-subheader>
        <v-btn
          color="secondary"
          @click="openCreateDialog"
          prepend-icon="mdi-plus"
          size="small"
          variant="tonal"
        >
          New
        </v-btn>
        <v-btn
          color="primary"
          @click="openUpdateDialog"
          :disabled="!activeBlueprintId"
          variant="tonal"
          size="small"
          prepend-icon="mdi-pencil"
        >
          Edit
        </v-btn>

        <v-btn
          color="success"
          @click="handleSave"
          :disabled="!activeBlueprintId"
          :loading="isSaving"
          variant="flat"
          size="small"
          prepend-icon="mdi-content-save"
        >
          Save
        </v-btn>
      </div>

      <v-alert
        v-if="listErrorMessage"
        type="error"
        density="compact"
        class="mb-2"
        >{{ listErrorMessage }}</v-alert
      >
      <!-- Empty State -->
      <v-list-item
        v-if="
          !isLoadingList && blueprintsList.length === 0 && !listErrorMessage
        "
      >
        <v-list-item-title class="text-caption"
          >No saved blueprints found.</v-list-item-title
        >
      </v-list-item>

      <!-- Blueprint List -->
      <v-list
        v-if="!isLoadingList && blueprintsList.length > 0"
        density="compact"
        nav
        class="mb-2"
        style="
          max-height: 250px;
          overflow-y: auto;
          border: 1px solid #eee;
          border-radius: 4px;
        "
      >
        <v-list-item
          v-for="blueprint in blueprintsList"
          :key="blueprint.id"
          @click="loadBlueprint(blueprint)"
          :active="blueprint.id === activeBlueprintId"
          color="primary"
        >
          <v-list-item-title>{{ blueprint.name }}</v-list-item-title>
          <v-list-item-subtitle>
            Floor {{ blueprint.floor }} | {{ blueprint.width }}x{{
              blueprint.height
            }}
          </v-list-item-subtitle>
          <template v-slot:append>
            <v-icon size="small">
              {{
                blueprint.id === activeBlueprintId
                  ? "mdi-select-inverse"
                  : "mdi-select"
              }}
            </v-icon>
          </template>
        </v-list-item>
      </v-list>

      <!-- <div v-else class="px-4 mb-4 text-caption text-grey-darken-1">
        Select a blueprint to see details, or create a new one.
      </div> -->

      <!-- <v-row class="mb-4" dense>
        <v-col>
          <v-btn
            color="primary"
            block
            @click="openUpdateDialog"
            :disabled="!activeBlueprintId"
            variant="tonal"
            prepend-icon="mdi-pencil"
          >
            Edit Details
          </v-btn>
        </v-col>
        <v-col>
          <v-btn
            color="success"
            block
            @click="handleSave"
            :disabled="!activeBlueprintId"
            :loading="isSaving"
            variant="flat"
            prepend-icon="mdi-content-save"
          >
            Save
          </v-btn>
        </v-col>
      </v-row> -->

      <div class="d-flex justify-space-between align-center mb-2">
        <v-list-subheader class="pa-0">
          {{
            currentTool === "placeComponent"
              ? "Select Item"
              : "Select Wall Type"
          }}
        </v-list-subheader>

        <v-btn-group
          density="compact"
          variant="tonal"
          :disabled="!activeBlueprintId"
        >
          <v-btn
            v-if="currentTool === 'placeComponent' && activeBlueprintId"
            @click="toggleComponentRotation"
            variant="tonal"
            color="cyan-darken-2"
            size="small"
            title="Toggle rotation"
          >
            <v-icon>{{
              isPlacingHorizontal ? "mdi-arrow-up-down" : "mdi-arrow-left-right"
            }}</v-icon>
          </v-btn>
          <v-btn
            @click="setTool('drawWall')"
            :color="currentTool === 'drawWall' ? 'indigo' : 'grey-lighten-1'"
            size="small"
            title="Draw Wall"
          >
            <v-icon icon="mdi-pencil"></v-icon>
          </v-btn>
          <v-btn
            @click="setTool('placeComponent')"
            :color="
              currentTool === 'placeComponent' ? 'cyan' : 'grey-lighten-1'
            "
            size="small"
            title="Place Item"
          >
            <v-icon icon="mdi-plus-box"></v-icon>
          </v-btn>
        </v-btn-group>
      </div>

      <v-list-subheader v-if="currentTool === 'placeComponent'"
        >Component Type</v-list-subheader
      >
      <v-select
        v-if="currentTool === 'placeComponent'"
        v-model="selectedComponentCategory"
        :items="componentCategoryOptions"
        item-title="title"
        item-value="value"
        label="Filter by category"
        density="compact"
        clearable
        class="mb-4"
      />

      <v-select
        v-if="currentTool === 'drawWall'"
        v-model="selectedBuildingStructureCategory"
        :items="buildingStructureCategoryOptions"
        item-title="title"
        item-value="value"
        label="Filter by structure category"
        density="compact"
        clearable
        class="mb-4"
      />
      <v-autocomplete
        v-show="currentTool === 'drawWall'"
        v-model="currentBuildingStructureId"
        :items="buildingStructureTypes"
        item-title="name"
        item-value="id"
        label="Search for a wall type"
        density="compact"
        class="mb-4"
        :loading="isLoadingStructureTypes"
        :error-messages="structureTypesError"
        auto-select-first
      >
        <template v-slot:item="{ props, item }">
          <v-list-item v-bind="props" :title="item.raw.name">
            <template v-slot:prepend>
              <div
                class="mr-3"
                :style="{
                  backgroundColor: item.raw.color,
                  width: '24px',
                  height: '4px',
                  borderRadius: '2px',
                }"
              ></div>
            </template>
          </v-list-item>
        </template>

        <template v-slot:selection="{ item }">
          <div class="d-flex align-center">
            <div
              class="mr-2"
              :style="{
                backgroundColor: item.raw.color,
                width: '20px',
                height: '4px',
                borderRadius: '2px',
              }"
            ></div>
            <span>{{ item.raw.name }}</span>
          </div>
        </template>
      </v-autocomplete>

      <v-autocomplete
        v-show="currentTool === 'placeComponent'"
        v-model="currentComponentId"
        :items="componentTypes"
        item-title="name"
        item-value="id"
        label="Search and select component"
        density="compact"
        class="mb-4"
        :loading="isLoadingComponentTypes"
        :error-messages="componentTypesError"
        auto-select-first
      >
        <template v-slot:item="{ props, item }">
          <v-list-item v-bind="props" :title="item.raw.name">
            <template v-slot:prepend>
              <v-avatar
                :color="item.raw.color"
                size="x-small"
                class="mr-2"
              ></v-avatar>
            </template>
          </v-list-item>
        </template>
        <template v-slot:selection="{ item }">
          <div class="d-flex align-center">
            <v-avatar
              :color="item.raw.color"
              size="x-small"
              class="mr-2"
            ></v-avatar>
            <span>{{ item.raw.name }}</span>
          </div>
        </template>
      </v-autocomplete>
      <div class="d-flex justify-space-between align-center mb-4">
        <v-btn
          color="pink"
          @click="clearFloorplan"
          size="small"
          :disabled="!activeBlueprintId"
        >
          <v-icon start icon="mdi-delete"></v-icon> Clear All
        </v-btn>

        <v-btn
          :color="
            currentTool === 'eraseWall' ? 'red-lighten-2' : 'grey-lighten-1'
          "
          @click="
            setTool(currentTool === 'eraseWall' ? 'drawWall' : 'eraseWall')
          "
          variant="flat"
          size="small"
          :disabled="!activeBlueprintId"
        >
          <v-icon
            start
            :icon="currentTool === 'eraseWall' ? 'mdi-pencil' : 'mdi-eraser'"
          ></v-icon>

          {{ currentTool === "eraseWall" ? "Stop Erase" : "Erase" }}
        </v-btn>
      </div>

      <v-list-subheader>Room Areas</v-list-subheader>
      <div class="px-4">
        <v-list v-if="rooms.length > 0" density="compact">
          <v-list-item
            v-for="room in rooms"
            :key="room.id"
            class="mb-2 border-sm rounded-lg"
          >
            <v-list-item-title class="font-weight-medium">
              {{ room.name }}
              <span class="font-weight-regular text-grey-darken-1 ml-2">
                ({{ room.squareMeters }} m²)
              </span>
            </v-list-item-title>

            <template v-slot:append>
              <v-select
                v-model="room.roomType"
                :items="roomTypeItems"
                item-title="title"
                item-value="value"
                density="compact"
                hide-details
                variant="plain"
                class="mb-3"
                style="max-width: 35px"
                @update:modelValue="renumberAllRooms"
                @click.stop
              >
                <template v-slot:selection="{ item }"></template>
              </v-select>
            </template>
          </v-list-item>
        </v-list>
        <v-list-item
          v-if="rooms.length === 0 && activeBlueprintId"
          class="pa-0"
        >
          <v-list-item-title class="text-caption text-grey"
            >No rooms found</v-list-item-title
          >
        </v-list-item>
      </div>
    </Teleport>

    <v-dialog v-model="isDetailsDialogVisible" max-width="800" persistent>
      <v-card>
        <v-card-title class="pa-4">
          <span class="text-h5">{{
            activeBlueprintId ? "Edit Blueprint" : "Create New Blueprint"
          }}</span>
        </v-card-title>

        <v-card-text>
          <v-form ref="detailsFormRef">
            <v-list-subheader>Details</v-list-subheader>
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

            <v-list-subheader>Canvas size (in cells)</v-list-subheader>
            <v-row>
              <v-col cols="6">
                <v-text-field
                  v-model.number="canvasWidthCells"
                  label="Width"
                  type="number"
                  min="10"
                  max="100"
                  density="compact"
                ></v-text-field>
              </v-col>
              <v-col cols="6">
                <v-text-field
                  v-model.number="canvasHeightCells"
                  label="Height"
                  type="number"
                  min="10"
                  max="100"
                  density="compact"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="isDetailsDialogVisible = false"
            >Cancel</v-btn
          >
          <v-btn
            color="success"
            variant="flat"
            :loading="isSaving"
            @click="handleSave({ validate: true, closeDialog: true })"
            >Save Blueprint</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="3000">
      {{ snackbarText }}
    </v-snackbar>
  </v-container>
</template>

<script setup>
import { ref, onMounted, watch, nextTick, computed } from "vue";
import { useTheme } from "vuetify";
import { useRoute } from "vue-router";
import { FONT_ROOM } from "@/configuration/drawing.js";
import { apiFetch } from "../components/api/auth.js";
import componentCategoryOptions from "../assets/enums/componentCategoryOptions.json";
import buildingStructureCategoryOptions from "../assets/enums/buildingStructureCategoryOptions.json";
import { generateUUID } from "@/components/floorplanner/UuidGenerator.js";

const theme = useTheme();
const route = useRoute();

// project id from url
const projectId = ref(route.params.projectId);
const projectName = ref();

// config
const GRID_SIZE = 20;
const WALL_THICKNESS = 5;
const GRID_COLOR = "#e5e7eb";
const DEFAULT_WALL_COLOR = theme.global.current.value.colors.primary;
const ERASE_HIGHLIGHT_COLOR = "#ef4444";
const DEFAULT_COMPONENT_COLOR = "#9e9e9e";

// Vue Reactive States
const walls = ref([]);
const placedComponents = ref([]);
const isDrawing = ref(false);
const startPoint = ref({ x: 0, y: 0 });
const tempEndPoint = ref({ x: 0, y: 0 });
const currentTool = ref("drawWall");
const canvasRef = ref(null);
const viewportRef = ref(null);
const canvasScale = ref(1);
const showCanvas = ref(false);
const canvasWidthCells = ref(40);
const canvasHeightCells = ref(40);
const finalCanvasSize = ref({ width: 0, height: 0 });
const ERASE_TOLERANCE = 10;
const hoveredWall = ref(null);
const hoveredComponent = ref(null);
const hoverPoint = ref(null);
const isPlacingHorizontal = ref(true);

// Blueprint form fields state
const blueprintName = ref("");
const blueprintDescription = ref("");
const maxOccupancy = ref(0);
const floor = ref(1);
const activeBlueprintId = ref(null);

// dialog and form states
const isDetailsDialogVisible = ref(false);
const detailsFormRef = ref(null);

// snackbar states
const snackbar = ref(false);
const snackbarText = ref("");
const snackbarColor = ref("success");

// API Logic Integration
const CORS_PROXY_URL = "https://corsproxy.io/?";
const BLUEPRINTS_API_URL = "http://planstack.dk/api/blueprints";
const BUILDING_STRUCTURES_API_URL =
  "http://planstack.dk/api/buildingstructures";
const COMPONENTS_API_URL = "http://planstack.dk/api/components";
const PROJECTS_API_URL = "http://planstack.dk/api/projects";

//building structure states
const buildingStructureTypes = ref([]);
const isLoadingStructureTypes = ref(false);
const structureTypesError = ref(null);
const currentBuildingStructureId = ref(1);

// component states
const componentTypes = ref([]);
const isLoadingComponentTypes = ref(false);
const componentTypesError = ref(null);
const currentComponentId = ref(null);

// room enums
const roomTypeItems = ref([
  { title: "Bathroom", value: 0 },
  { title: "Bedroom", value: 1 },
  { title: "Living Room", value: 2 },
  { title: "Kitchen", value: 3 },
  { title: "Dining Room", value: 4 },
  { title: "Office", value: 5 },
]);

// room calc states
const rooms = ref([]);
const isCalculatingRooms = ref(false);
const showRoomLabels = ref(true);
const roomGrid = ref(null);

const COLOR_PALETTE = [
  theme.global.current.value.colors.primary,
  "#FF5733",
  "#33FF57",
  "#3357FF",
  "#FF33A1",
  "#33FFA1",
  "#A133FF",
  "#FF8F33",
];

//saving states
const isSaving = ref(false);

// loading states
const blueprintsList = ref([]);
const isLoadingList = ref(false);
const listErrorMessage = ref("");

const canvasStyle = computed(() => ({
  transform: `scale(${canvasScale.value})`,
  transformOrigin: "center center",
  touchAction: "none",
  cursor: "crosshair",
}));

const showSnackbar = (text, color = "success") => {
  snackbarText.value = text;
  snackbarColor.value = color;
  snackbar.value = true;
};

const openCreateDialog = () => {
  // reset fields
  activeBlueprintId.value = null;
  blueprintName.value = "New Blueprint";
  blueprintDescription.value = "";
  maxOccupancy.value = 0;
  floor.value = 1;
  canvasWidthCells.value = 40;
  canvasHeightCells.value = 40;
  walls.value = [];
  placedComponents.value = [];
  rooms.value = [];
  roomGrid.value = null;

  // clearing canvas if showing
  if (showCanvas.value) {
    createCanvas();
  }

  isDetailsDialogVisible.value = true;
  nextTick(() => {
    detailsFormRef.value?.resetValidation();
  });
};

const openUpdateDialog = () => {
  isDetailsDialogVisible.value = true;
  nextTick(() => {
    detailsFormRef.value?.resetValidation();
  });
};

const toggleComponentRotation = () => {
  isPlacingHorizontal.value = !isPlacingHorizontal.value;
  draw();
};

const selectedBuildingStructureCategory = ref(null);
watch(selectedBuildingStructureCategory, (newCategory) => {
  fetchBuildingStructureTypes(newCategory);
});

// get building structures
const fetchBuildingStructureTypes = async (category = null) => {
  isLoadingStructureTypes.value = true;
  structureTypesError.value = null;
  try {
    let url = BUILDING_STRUCTURES_API_URL;
    if (category !== null) {
      url += `?Category=${category}`;
    }
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;
    const response = await apiFetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });
    if (!response.ok) {
      throw new Error(`API returned error: Status ${response.status}`);
    }
    const data = await response.json();

    // assign colors to each
    buildingStructureTypes.value = data.entities.map((type, index) => ({
      ...type,
      color: type.color || COLOR_PALETTE[index % COLOR_PALETTE.length],
    }));

    // default drawing ID
    if (buildingStructureTypes.value.length > 0) {
      currentBuildingStructureId.value = buildingStructureTypes.value[0].id;
    }
  } catch (error) {
    console.error("Error fetching building structure types:", error);
    structureTypesError.value = `Failed to load wall types: ${error.message}`;
  } finally {
    isLoadingStructureTypes.value = false;
  }
};

const selectedComponentCategory = ref(null);
watch(selectedComponentCategory, (newCategory) => {
  fetchComponentTypes(newCategory);
});

// get components
const fetchComponentTypes = async (category = null) => {
  isLoadingComponentTypes.value = true;
  componentTypesError.value = null;
  try {
    let url = COMPONENTS_API_URL;
    if (category !== null) {
      url += `?Category=${category}`;
    }
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;
    const response = await apiFetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });
    if (!response.ok) {
      throw new Error(`API returned error: Status ${response.status}`);
    }
    const data = await response.json();

    // assigning colors
    componentTypes.value = data.entities.map((type, index) => ({
      ...type,
      color: type.color || COLOR_PALETTE[index % COLOR_PALETTE.length],
    }));
  } catch (error) {
    console.error("Error fetching component types:", error);
    componentTypesError.value = `Failed to load component types: ${error.message}`;
  } finally {
    isLoadingComponentTypes.value = false;
  }
};

const getColorForStructureId = (id) => {
  const structure = buildingStructureTypes.value.find((s) => s.id === id);
  return structure ? structure.color : DEFAULT_WALL_COLOR;
};

const getComponentDetails = (id) => {
  const comp = componentTypes.value.find((c) => c.id === id);
  return comp
    ? {
        color: comp.color || DEFAULT_COMPONENT_COLOR,
        width: comp.width || 1,
        height: comp.height || 1,
      }
    : {
        color: DEFAULT_COMPONENT_COLOR,
        width: 1,
        height: 1,
      };
};

//fetch project
const fetchProjectDetails = async () => {
  if (!projectId.value) {
    projectName.value = "Unknown Project";
    return;
  }

  try {
    const url = `${PROJECTS_API_URL}/${projectId.value}`;
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await apiFetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      throw new Error(`API error: Status ${response.status}`);
    }
    const data = await response.json();
    projectName.value = data.name; //project name
  } catch (error) {
    console.error("Error fetching project details:", error);
    projectName.value = "Unknown Project";
  }
};

// get blueprints
const fetchBlueprints = async () => {
  isLoadingList.value = true;
  listErrorMessage.value = "";
  try {
    const blueprintApiUrl = `${BLUEPRINTS_API_URL}?projectId=${projectId.value}`;
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(blueprintApiUrl)}`;

    const response = await apiFetch(proxiedUrl, {
      method: "GET",
      headers: {
        Host: "planstack.dk",
      },
    });

    if (!response.ok) {
      throw new Error(`API returned error: Status ${response.status}`);
    }

    const data = await response.json();

    blueprintsList.value = data.entities;

    if (data.entities.length === 0) {
      showSnackbar("No saved blueprints found for this project.", "info");
    }
  } catch (error) {
    console.error("Error fetching blueprints:", error);
    listErrorMessage.value = `Error fetching list: ${error.message}`;
  } finally {
    isLoadingList.value = false;
  }
};

// post/put blueprints
const saveBlueprintToAPI = async (blueprintData) => {
  isSaving.value = true;

  const isUpdating = !!blueprintData.id;
  const method = isUpdating ? "PUT" : "POST";
  const url = isUpdating
    ? `${BLUEPRINTS_API_URL}/save/${blueprintData.id}`
    : BLUEPRINTS_API_URL;

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await apiFetch(proxiedUrl, {
      method: method,
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
        errorDetail = errorData.message || JSON.stringify(errorData);
      } catch (e) {}
      throw new Error(
        `API returned error: Status ${response.status}. Detail: ${errorDetail}`
      );
    }

    let data = null;
    if (response.status !== 204) {
      // 204 No Content is valid for PUT/POST
      try {
        data = await response.json();
        console.log("API Response:", data);
      } catch (e) {
        console.log("API Response: No JSON body returned.");
      }
    } else {
      console.log("API Response: 204 No Content");
    }

    showSnackbar(`Blueprint ${isUpdating ? "updated" : "saved"} successfully!`);

    // If creating, response data might have new ID
    if (!isUpdating && data && data.id) {
      activeBlueprintId.value = data.id;
      // update name if returned
      if (data.name) blueprintName.value = data.name;
    }

    // refresh blueprint list after saving
    await fetchBlueprints();
  } catch (error) {
    console.error("Error saving blueprint:", error);
    showSnackbar(
      `Error saving blueprint: ${error.message || "An unknown error occurred"}.`,
      "error"
    );
    throw error;
  } finally {
    isSaving.value = false;
  }
};

// snap coordinates to grid
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

const drawComponent = (
  ctx,
  x,
  y,
  widthCells,
  heightCells,
  color,
  isHorizontal,
  isGhost = false,
  isHoveredErase = false
) => {
  // dimension based on isHorisontal
  const w_px = (isHorizontal ? widthCells : heightCells) * GRID_SIZE;
  const h_px = (isHorizontal ? heightCells : widthCells) * GRID_SIZE;

  if (isHoveredErase) {
    ctx.fillStyle = ERASE_HIGHLIGHT_COLOR;
    ctx.strokeStyle = "#000";
  } else {
    ctx.fillStyle = color;
    ctx.strokeStyle = "#333";
  }

  ctx.globalAlpha = isGhost ? 0.5 : 1.0;
  ctx.lineWidth = 2;

  ctx.beginPath();
  ctx.rect(x, y, w_px, h_px);
  ctx.fill();
  ctx.stroke();
  ctx.globalAlpha = 1.0;
};

const draw = () => {
  const canvas = canvasRef.value;
  if (!canvas) return;
  const ctx = canvas.getContext("2d");

  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawGrid(ctx, canvas);

  // draw components
  placedComponents.value.forEach((comp) => {
    const isHovered = hoveredComponent.value && comp === hoveredComponent.value;
    const { color, width, height } = getComponentDetails(comp.componentId);
    drawComponent(
      ctx,
      comp.x,
      comp.y,
      width,
      height,
      color,
      comp.isHorizontal,
      false,
      isHovered
    );
  });

  // draw walls
  walls.value.forEach((wall) => {
    const isHovered = hoveredWall.value && wall === hoveredWall.value;
    const color = isHovered
      ? ERASE_HIGHLIGHT_COLOR
      : getColorForStructureId(wall.buildingStructureId);
    drawWall(ctx, wall.startX, wall.startY, wall.endX, wall.endY, color);
  });

  // draw temp wall (if drawing)
  if (isDrawing.value && currentTool.value === "drawWall") {
    const tempColor = getColorForStructureId(currentBuildingStructureId.value);
    drawWall(
      ctx,
      startPoint.value.x,
      startPoint.value.y,
      tempEndPoint.value.x,
      tempEndPoint.value.y,
      tempColor
    );
  }

  // draw comp preview
  if (
    hoverPoint.value &&
    currentTool.value === "placeComponent" &&
    currentComponentId.value
  ) {
    const { color, width, height } = getComponentDetails(
      currentComponentId.value
    );
    drawComponent(
      ctx,
      hoverPoint.value.x,
      hoverPoint.value.y,
      width,
      height,
      color,
      isPlacingHorizontal.value,
      true //ghosted
    );
  }

  // draw room labels
  if (showRoomLabels.value) {
    rooms.value.forEach((room) => {
      const canvasX = ((room.labelX - 1) / 2) * GRID_SIZE + GRID_SIZE / 2;
      const canvasY = ((room.labelY - 1) / 2) * GRID_SIZE + GRID_SIZE / 2;

      drawRoomLabel(ctx, room.name, canvasX, canvasY);
    });
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

const isPointNearComponent = (px, py, component) => {
  const { width, height } = getComponentDetails(component.componentId);

  // dimensions based on rotation
  const w_px = (component.isHorizontal ? width : height) * GRID_SIZE;
  const h_px = (component.isHorizontal ? height : width) * GRID_SIZE;

  const x1 = component.x;
  const x2 = component.x + w_px;
  const y1 = component.y;
  const y2 = component.y + h_px;

  return px >= x1 && px <= x2 && py >= y1 && py <= y2;
};

// event handlers
const getCanvasCoords = (event) => {
  const canvas = canvasRef.value;
  if (!canvas) return { x: 0, y: 0 };

  let clientX = event.clientX;
  let clientY = event.clientY;

  if (event.touches) {
    clientX = event.touches[0].clientX;
    clientY = event.touches[0].clientY;
  }

  const rect = canvas.getBoundingClientRect();

  const x = (clientX - rect.left) / canvasScale.value;
  const y = (clientY - rect.top) / canvasScale.value;

  return { x, y };
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
    startPoint.value = { x, y };
    isDrawing.value = true;
  } else if (currentTool.value === "placeComponent") {
    if (!currentComponentId.value) {
      showSnackbar("Please select a component type first.", "error");
      return;
    }
    const newComponent = {
      x: snappedPoint.x,
      y: snappedPoint.y,
      componentId: currentComponentId.value,
      isHorizontal: isPlacingHorizontal.value,
    };
    placedComponents.value.push(newComponent);
    draw();
  }
};

const handleMouseUp = () => {
  if (!isDrawing.value) return;

  isDrawing.value = false;

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
        buildingStructureId: currentBuildingStructureId.value,
      };

      const updatedWalls = [...walls.value, newWall];
      walls.value = updatedWalls;
    }
  } else if (currentTool.value === "eraseWall") {
    // erase logic - check if the cursor is near any wall or component
    const clickX = startPoint.value.x;
    const clickY = startPoint.value.y;

    const wallIndexToErase = walls.value.findIndex((wall) =>
      isPointNearWall(clickX, clickY, wall)
    );

    if (wallIndexToErase !== -1) {
      // Remove the wall
      walls.value = walls.value.filter(
        (_, index) => index !== wallIndexToErase
      );
      showSnackbar("Wall erased.");
    } else {
      const compIndexToErase = placedComponents.value.findIndex((comp) =>
        isPointNearComponent(clickX, clickY, comp)
      );
      // remove component if it is not a wall
      if (compIndexToErase !== -1) {
        placedComponents.value = placedComponents.value.filter(
          (_, index) => index !== compIndexToErase
        );
        showSnackbar("Component erased.");
      } else {
        showSnackbar("No wall or component found to erase.", "info");
      }
    }
  }
  // room calc after any change
  calculateRoomAreas();
  draw();
};

const handleMouseMove = (event) => {
  const { x, y } = getCanvasCoords(event);
  const snappedPoint = { x: snapToGrid(x), y: snapToGrid(y) };

  if (!isDrawing.value) {
    let needsRedraw = false;
    // hover highlight for erase
    if (currentTool.value === "eraseWall") {
      const wallUnderCursor = walls.value.find((wall) =>
        isPointNearWall(x, y, wall)
      );
      const compUnderCursor = placedComponents.value.find((comp) =>
        isPointNearComponent(x, y, comp)
      );

      let hWall = null;
      let hComp = null;
      // prioritise walls
      if (wallUnderCursor) {
        hWall = wallUnderCursor;
      } else if (compUnderCursor) {
        hComp = compUnderCursor;
      }

      if (hoveredWall.value !== hWall || hoveredComponent.value !== hComp) {
        hoveredWall.value = hWall;
        hoveredComponent.value = hComp;
        needsRedraw = true;
      }
    }

    // component placement preview
    if (currentTool.value === "placeComponent") {
      if (
        !hoverPoint.value ||
        hoverPoint.value.x !== snappedPoint.x ||
        hoverPoint.value.y !== snappedPoint.y
      ) {
        hoverPoint.value = snappedPoint;
        needsRedraw = true;
      }
    } else {
      if (hoverPoint.value !== null) {
        hoverPoint.value = null;
        needsRedraw = true;
      }
    }

    if (needsRedraw) {
      draw();
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

    // constrain to 90 degrees angles
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
  let needsRedraw = false;
  if (hoveredWall.value) {
    hoveredWall.value = null;
    needsRedraw = true;
  }
  if (hoveredComponent.value) {
    hoveredComponent.value = null;
    needsRedraw = true;
  }
  if (hoverPoint.value) {
    hoverPoint.value = null;
    needsRedraw = true;
  }

  if (needsRedraw) {
    draw();
  }

  if (isDrawing.value) {
    handleMouseUp();
  }
};

const clearFloorplan = () => {
  walls.value = [];
  placedComponents.value = [];
  rooms.value = [];
  roomGrid.value = null;
  showSnackbar("Floorplan cleared.", "info");
  draw();
};

const setTool = (tool) => {
  currentTool.value = tool;
  hoveredWall.value = null;
  hoveredComponent.value = null;
  draw();

  if (tool === "drawWall") {
    showSnackbar(
      "Drawing tool selected. Click and drag to create walls.",
      "info"
    );
  } else if (tool === "eraseWall") {
    showSnackbar(
      "Eraser tool selected. Click on a wall or component to remove it.",
      "info"
    );
  } else if (tool === "placeComponent") {
    showSnackbar(
      "Place Component tool selected. Click on the grid to place.",
      "info"
    );
  } else {
    showSnackbar(`Tool ${tool} selected.`, "info");
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
};

const createCanvas = (closeDialog = false) => {
  const widthCells = Number(canvasWidthCells.value);
  const heightCells = Number(canvasHeightCells.value);

  if (
    isNaN(widthCells) ||
    isNaN(heightCells) ||
    widthCells < 10 ||
    heightCells < 10
  ) {
    showSnackbar("Please enter valid canvas dimensions (min 10x10).", "error");
    return;
  }

  finalCanvasSize.value = {
    width: widthCells * GRID_SIZE,
    height: heightCells * GRID_SIZE,
  };

  showCanvas.value = true;
  rooms.value = [];
  roomGrid.value = null;

  nextTick(() => {
    if (!viewportRef.value || !canvasRef.value) {
      console.error("Canvas or Viewport ref not found!");
      return;
    }

    // container and canvas size
    const viewportWidth = viewportRef.value.clientWidth;
    const viewportHeight = viewportRef.value.clientHeight;

    const canvasWidth = finalCanvasSize.value.width;
    const canvasHeight = finalCanvasSize.value.height;

    // calculate scale to fit canvas in viewport
    const scaleX = viewportWidth / canvasWidth;
    const scaleY = viewportHeight / canvasHeight;

    const newScale = Math.min(scaleX, scaleY) * 0.95;
    canvasScale.value = newScale;

    setupCanvasAndListeners();
  });

  if (closeDialog) {
    isDetailsDialogVisible.value = false;
    showSnackbar("Canvas size applied");
  }
};

//converting canvas coordinates to gridcells for saving building strucutres to blueprint
const mapWallsToBlueprintPayload = () => {
  return walls.value.map((wall) => {
    const startCellX = wall.startX / GRID_SIZE;
    const startCellY = wall.startY / GRID_SIZE;
    const endCellX = wall.endX / GRID_SIZE;
    const endCellY = wall.endY / GRID_SIZE;

    const width = Math.abs(endCellX - startCellX);
    const height = Math.abs(endCellY - startCellY);

    const startingPositionX = Math.min(startCellX, endCellX);
    const startingPositionY = Math.min(startCellY, endCellY);
    const buildingStructure = buildingStructureTypes.value.find(
      (b) => b.id === wall.buildingStructureId
    );

    return {
      height: height,
      width: width,
      startingPositionX: startingPositionX,
      startingPositionY: startingPositionY,
      totalPrice: 0,
      blueprintId: activeBlueprintId.value || 0,
      buildingStructure: buildingStructure || null,
      buildingStructureId: wall.buildingStructureId || 1,
    };
  });
};

// rooms and components payload
const mapRoomsAndComponentsToPayload = () => {
  const componentsPayload = [];
  const roomsPayload = rooms.value.map((r) => ({
    id: r.id,
    name: r.name || "",
    description: "",
    squareMeters: r.squareMeters,
    roomType: r.roomType,
    blueprintId: activeBlueprintId.value || 0,
  }));

  const grid = roomGrid.value;

  for (const comp of placedComponents.value) {
    // comp payload
    const componentApiData = {
      startingPositionX: comp.x / GRID_SIZE,
      startingPositionY: comp.y / GRID_SIZE,
      isHorizontal: comp.isHorizontal,
      blueprintId: activeBlueprintId.value || 0,
      componentId: comp.componentId,
      roomId: null,
    };

    // find room id comp is in
    const cellX = Math.floor(comp.x / GRID_SIZE);
    const cellY = Math.floor(comp.y / GRID_SIZE);
    const gridX = cellX * 2 + 1;
    const gridY = cellY * 2 + 1;

    if (grid[gridY] && grid[gridY][gridX] !== undefined) {
      const roomId = grid[gridY][gridX];

      if (typeof roomId === "string") {
        componentApiData.roomId = roomId;
      }
    }
    componentsPayload.push(componentApiData);
  }

  return { components: componentsPayload, rooms: roomsPayload };
};

const mapBlueprintStructuresToWalls = (structures) => {
  if (!structures) return [];
  return structures.map((structure) => {
    const startX = structure.startingPositionX * GRID_SIZE;
    const startY = structure.startingPositionY * GRID_SIZE;
    const endX = (structure.startingPositionX + structure.width) * GRID_SIZE;
    const endY = (structure.startingPositionY + structure.height) * GRID_SIZE;

    // we get buildingstructure id from the nested object (maybe change?)
    const buildingStructureId = structure.buildingStructure
      ? structure.buildingStructure.id
      : 1;

    if (structure.width > 0) {
      return {
        startX,
        startY,
        endX,
        endY: startY,
        buildingStructureId,
      };
    } else {
      return {
        startX,
        startY,
        endX: startX,
        endY,
        buildingStructureId,
      };
    }
  });
};

const mapBlueprintComponentsToPlaced = (componentsData) => {
  if (!componentsData) return [];
  return componentsData.map((comp) => {
    const x = comp.startingPositionX * GRID_SIZE;
    const y = comp.startingPositionY * GRID_SIZE;
    // component id from nested object
    const componentId = comp.component ? comp.component.id : 1;
    return {
      x,
      y,
      componentId,
      isHorizontal: comp.isHorizontal ?? true,
    };
  });
};

const handleSave = async ({ validate = false, closeDialog = false } = {}) => {
  if (validate) {
    const { valid } = await detailsFormRef.value.validate();
    if (!valid) {
      showSnackbar("Please fill in all required fields.", "error");
      return;
    }
  }

  // if it can save
  if (!validate && !activeBlueprintId.value) {
    showSnackbar("Cannot quick save. Please load a blueprint first.", "error");
    return;
  }
  if (!showCanvas.value) {
    if (validate) {
      createCanvas();
    } else {
      showSnackbar("Canvas is not active. Load a blueprint first.", "error");
      return;
    }
  }
  calculateRoomAreas();
  await nextTick();

  const buildingStructures = mapWallsToBlueprintPayload();

  const { components, rooms } = mapRoomsAndComponentsToPayload();

  const apiPayload = {
    ...(activeBlueprintId.value && { id: activeBlueprintId.value }),

    name: blueprintName.value,
    description: blueprintDescription.value,
    maxOccupancy: maxOccupancy.value,
    floor: floor.value,

    height: canvasHeightCells.value,
    width: canvasWidthCells.value,

    buildingStructures: buildingStructures,
    components: components,
    rooms: rooms,
    projectId: projectId.value,
    standards: [],
  };
  console.log("Saving to api:", JSON.stringify(apiPayload, null, 2));

  //api call
  try {
    await saveBlueprintToAPI(apiPayload);

    if (closeDialog) {
      isDetailsDialogVisible.value = false;
    }
  } catch (error) {
    console.error("save failed");
  }
};

const loadBlueprint = (blueprint) => {
  console.log("Loading blueprint:", blueprint);

  // form fields to set
  activeBlueprintId.value = blueprint.id;
  blueprintName.value = blueprint.name;
  blueprintDescription.value = blueprint.description || "";
  maxOccupancy.value = blueprint.maxOccupancy || 0;
  floor.value = blueprint.floor || 1;
  canvasWidthCells.value = blueprint.width || 40;
  canvasHeightCells.value = blueprint.height || 40;
  // rooms.value = [];
  roomGrid.value = null;

  //to create or resize canvas
  createCanvas();

  if (blueprint.buildingStructures && blueprint.buildingStructures.length > 0) {
    walls.value = mapBlueprintStructuresToWalls(blueprint.buildingStructures);
    showSnackbar(`Loaded blueprint: ${blueprint.name}`);
  } else {
    // no structures then clear walls
    walls.value = [];
    showSnackbar(
      `Loaded blueprint: ${blueprint.name} (No walls found)`,
      "info"
    );
  }

  if (blueprint.components && blueprint.components.length > 0) {
    placedComponents.value = mapBlueprintComponentsToPlaced(
      blueprint.components
    );
  } else {
    placedComponents.value = [];
  }

  if (blueprint.rooms && blueprint.rooms.length > 0) {
    rooms.value = blueprint.rooms.map((r, index) => ({
      id: r.id || generateUUID(),
      name: r.name || `Room ${index + 1}`,
      squareMeters: r.squareMeters,
      roomType: r.roomType ?? 0,
      labelX: 0,
      labelY: 0,
    }));
  } else {
    rooms.value = [];
  }

  nextTick(() => {
    draw();
    updateRoomGridAndLabels();
  });
};

// const onFindRoomsHover = () => {
//   if (rooms.value.length > 0) {
//     showRoomLabels.value = true;
//     draw();
//   }
// };

// const onFindRoomsLeave = () => {
//   if (showRoomLabels.value) {
//     showRoomLabels.value = false;
//     draw();
//   }
// };

const drawRoomLabel = (ctx, text, x, y) => {
  ctx.fillStyle = theme.global.current.value.colors.primary;
  ctx.font = FONT_ROOM;
  ctx.textAlign = "center";
  ctx.textBaseline = "middle";
  ctx.fillText(text, x, y);
};

// roomtypes
const getRoomTypeName = (value) => {
  const item = roomTypeItems.value.find((item) => item.value === value);
  return item ? item.title : "N/A";
};

// room nametype & number
const renumberAllRooms = () => {
  if (rooms.value.length === 0) return;

  // get room types present
  const roomTypes = [...new Set(rooms.value.map((r) => r.roomType))];

  roomTypes.forEach((type) => {
    const roomTypeName = getRoomTypeName(type);

    // find room of this type
    const roomsOfType = rooms.value
      .filter((r) => r.roomType === type)
      .sort((a, b) => a.labelY - b.labelY || a.labelX - b.labelX);

    // assign name & number
    roomsOfType.forEach((room, index) => {
      room.name = `${roomTypeName} ${index + 1}`;
    });
  });
};

const calculateRoomAreas = () => {
  if (walls.value.length === 0) {
    rooms.value = [];
    roomGrid.value = null;
    draw();
    return;
  }

  isCalculatingRooms.value = true;

  const oldRooms = [...rooms.value];
  const oldGrid = roomGrid.value;
  const usedOldIds = new Set(); // track old rooms

  const width = canvasWidthCells.value;
  const height = canvasHeightCells.value;

  // double grid for squaremeter calc
  const gridWidth = width * 2 + 1;
  const gridHeight = height * 2 + 1;
  const grid = Array(gridHeight)
    .fill(null)
    .map(() => Array(gridWidth).fill(0));

  // marking walls double grid
  walls.value.forEach((wall) => {
    const x1 = Math.round(wall.startX / GRID_SIZE) * 2;
    const y1 = Math.round(wall.startY / GRID_SIZE) * 2;
    const x2 = Math.round(wall.endX / GRID_SIZE) * 2;
    const y2 = Math.round(wall.endY / GRID_SIZE) * 2;

    // line on grid
    for (let y = Math.min(y1, y2); y <= Math.max(y1, y2); y++) {
      for (let x = Math.min(x1, x2); x <= Math.max(x1, x2); x++) {
        if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth) {
          grid[y][x] = 1; // 1 is wall
        }
      }
    }
  });

  floodFill(grid, 0, 0, -1, 0, gridWidth, gridHeight);

  const foundRooms = [];

  // Iterate over cells
  for (let y = 1; y < gridHeight; y += 2) {
    for (let x = 1; x < gridWidth; x += 2) {
      if (grid[y][x] === 0) {
        // check old grid for old room id
        const oldRoomId = oldGrid ? oldGrid[y]?.[x] : undefined;
        const oldRoom = oldRoomId
          ? oldRooms.find((r) => r.id === oldRoomId)
          : undefined;

        let roomIdToAssign;
        let roomType;
        let roomName;

        if (oldRoom && !usedOldIds.has(oldRoom.id)) {
          //old room
          roomIdToAssign = oldRoom.id;
          roomType = oldRoom.roomType;
          roomName = oldRoom.name;
          usedOldIds.add(oldRoom.id);
        } else {
          // new room or split old room
          roomIdToAssign = generateUUID();
          roomType = 0;
          roomName = "";
        }

        const roomData = floodFill(
          grid,
          x,
          y,
          roomIdToAssign,
          0,
          gridWidth,
          gridHeight
        );

        if (roomData.squareMeters > 0) {
          // finding center cell for showing room name
          const centerX = (roomData.minX + roomData.maxX) / 2;
          const centerY = (roomData.minY + roomData.maxY) / 2;

          let bestCell = [0, 0];
          let minDistance = Infinity;

          for (const cell of roomData.allCells) {
            const distance = Math.hypot(cell[0] - centerX, cell[1] - centerY);
            if (distance < minDistance) {
              minDistance = distance;
              bestCell = cell;
            }
          }

          foundRooms.push({
            id: roomIdToAssign,
            name: roomName,
            roomType: roomType,
            squareMeters: roomData.squareMeters,
            labelX: bestCell[0],
            labelY: bestCell[1],
          });
        }
      }
    }
  }

  rooms.value = foundRooms;
  roomGrid.value = grid;
  // renumber rooms
  renumberAllRooms();

  isCalculatingRooms.value = false;

  if (foundRooms.length > 0 && !oldRooms.length) {
    showSnackbar(`Found ${foundRooms.length} rooms!`);
  } else if (walls.value.length > 0 && foundRooms.length === 0) {
    showSnackbar("Could not find any rooms.", "error");
  }

  draw();
};

const updateRoomGridAndLabels = () => {
  if (walls.value.length === 0) {
    rooms.value = [];
    roomGrid.value = null;
    draw();
    return;
  }

  const width = canvasWidthCells.value;
  const height = canvasHeightCells.value;
  const gridWidth = width * 2 + 1;
  const gridHeight = height * 2 + 1;
  const grid = Array(gridHeight)
    .fill(null)
    .map(() => Array(gridWidth).fill(0));

  // mark walls on grid
  walls.value.forEach((wall) => {
    const x1 = Math.round(wall.startX / GRID_SIZE) * 2;
    const y1 = Math.round(wall.startY / GRID_SIZE) * 2;
    const x2 = Math.round(wall.endX / GRID_SIZE) * 2;
    const y2 = Math.round(wall.endY / GRID_SIZE) * 2;
    for (let y = Math.min(y1, y2); y <= Math.max(y1, y2); y++) {
      for (let x = Math.min(x1, x2); x <= Math.max(x1, x2); x++) {
        if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth) {
          grid[y][x] = 1; // 1 is wall
        }
      }
    }
  });
  floodFill(grid, 0, 0, -1, 0, gridWidth, gridHeight);

  // label for existing rooms
  const roomsToUpdate = [...rooms.value];
  const foundRoomBlobs = [];

  for (let y = 1; y < gridHeight; y += 2) {
    for (let x = 1; x < gridWidth; x += 2) {
      if (grid[y][x] === 0) {
        // room blob, dk id
        const tempId = generateUUID();
        const roomData = floodFill(
          grid,
          x,
          y,
          tempId,
          0,
          gridWidth,
          gridHeight
        );

        if (roomData.squareMeters > 0) {
          // center cell calc
          const centerX = (roomData.minX + roomData.maxX) / 2;
          const centerY = (roomData.minY + roomData.maxY) / 2;
          let bestCell = [0, 0];
          let minDistance = Infinity;
          for (const cell of roomData.allCells) {
            const distance = Math.hypot(cell[0] - centerX, cell[1] - centerY);
            if (distance < minDistance) {
              minDistance = distance;
              bestCell = cell;
            }
          }

          // blob center
          const labelX = bestCell[0];
          const labelY = bestCell[1];

          // check if any room matches this label position
          const matchingRoom = roomsToUpdate.find(
            (r) => r.labelX === labelX && r.labelY === labelY
          );

          if (matchingRoom) {
            // match loaded room, update grid with read id
            floodFill(
              grid,
              x,
              y,
              matchingRoom.id,
              tempId,
              gridWidth,
              gridHeight
            );
            matchingRoom.squareMeters = roomData.squareMeters;
          } else {
            // new room then add to room list
            const newRoom = {
              id: tempId,
              name: "New Room",
              roomType: 0,
              squareMeters: roomData.squareMeters,
              labelX: labelX,
              labelY: labelY,
            };
            roomsToUpdate.push(newRoom);
          }
        }
      }
    }
  }

  rooms.value = roomsToUpdate;
  roomGrid.value = grid;

  renumberAllRooms();
  draw();
};

const floodFill = (
  grid,
  startX,
  startY,
  newValue,
  targetValue,
  width,
  height
) => {
  if (grid[startY][startX] !== targetValue) {
    return { squareMeters: 0 };
  }

  let squareMeters = 0;
  let minX = Infinity,
    maxX = -Infinity,
    minY = Infinity,
    maxY = -Infinity;
  const allCells = [];
  const queue = [[startX, startY]];
  grid[startY][startX] = newValue;

  while (queue.length > 0) {
    const [x, y] = queue.shift();

    // checking if its a cell center
    if (x % 2 === 1 && y % 2 === 1) {
      squareMeters++;
      allCells.push([x, y]);

      // Update bounding box
      if (x < minX) minX = x;
      if (x > maxX) maxX = x;
      if (y < minY) minY = y;
      if (y > maxY) maxY = y;
    }

    // checking neighbors
    const neighbors = [
      [x + 1, y],
      [x - 1, y],
      [x, y + 1],
      [x, y - 1],
      [x + 1, y + 1],
      [x - 1, y - 1],
      [x + 1, y - 1],
      [x - 1, y + 1],
    ];

    for (const [nx, ny] of neighbors) {
      // checking bounds and target value
      if (
        nx >= 0 &&
        nx < width &&
        ny >= 0 &&
        ny < height &&
        grid[ny][nx] === targetValue
      ) {
        grid[ny][nx] = newValue;
        queue.push([nx, ny]);
      }
    }
  }
  return { squareMeters, minX, maxX, minY, maxY, allCells };
};

// recalculate rooms when walls change
watch(
  walls,
  () => {
    calculateRoomAreas();
  },
  { deep: true }
);
watch(placedComponents, draw, { deep: true });
watch(buildingStructureTypes, draw, { deep: true });
watch(componentTypes, draw, { deep: true });

watch(currentComponentId, (newId) => {
  if (newId) {
    const { width, height } = getComponentDetails(newId);
    // default rotation based on dimension
    isPlacingHorizontal.value = width >= height;
    draw();
  }
});

onMounted(() => {
  fetchProjectDetails();
  fetchBlueprints();
  fetchBuildingStructureTypes();
  fetchComponentTypes();
});
</script>

<style scoped>
/* testing viewport for canvas */
.canvas-viewport {
  width: 100%;
  max-width: 900px;
  height: 600px;
  border: 1px solid #e0e0e0;
  overflow: hidden;
  position: relative;
  margin: 0 auto;
}

#floorplanCanvas {
  border: 2px solid #cbd5e1;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  background-color: #ffffff;
  transform-origin: center center;
}
</style>
