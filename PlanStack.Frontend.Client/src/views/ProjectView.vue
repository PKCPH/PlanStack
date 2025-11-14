<template>
  <v-container class="py-10">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">Projects</h1>
    </div>

    <v-tabs v-model="tab" align-tabs="center" class="mb-6">
      <v-tab value="select">Select Project</v-tab>
      <v-tab value="create">Create New Project</v-tab>
    </v-tabs>

    <v-window v-model="tab">
      <v-window-item value="select">
        <v-sheet min-width="600" class="mx-auto">
          <v-list-subheader>Select a Project</v-list-subheader>

          <v-skeleton-loader
            v-if="isLoadingProjects"
            type="list-item-two-line@3"
          ></v-skeleton-loader>

          <v-alert
            v-if="projectsError"
            type="error"
            title="Error loading projects"
            :text="projectsError"
            class="mb-4"
          ></v-alert>

          <v-alert
            v-if="!isLoadingProjects && !projectsError && projects.length === 0"
            type="info"
            text="No projects found. You can create one in the 'Create New Project'"
          ></v-alert>

          <v-list
            v-if="!isLoadingProjects && projects.length > 0"
            lines="two"
            style="max-height: 400px; overflow-y: auto; border: 1px solid #eee"
          >
            <v-list-item
              v-for="project in projects"
              :key="project.id"
              :title="project.name"
              :subtitle="project.description || 'No description'"
              @click="selectProject(project)"
              ripple
            >
              <template v-slot:append>
                <v-list-item-subtitle class="mr-4">
                  {{ project.squareMeters }} m²
                </v-list-item-subtitle>

                <v-list-item-subtitle class="mr-4">
                  {{ getBuildingType(project.buildingType) }}
                </v-list-item-subtitle>

                <v-btn
                  color="grey-darken-1"
                  icon="mdi-pencil"
                  variant="text"
                  size="small"
                  class="mr-1"
                  @click.stop="openUpdateDialog(project)"
                ></v-btn>

                <v-btn
                  color="red-lighten-1"
                  icon="mdi-delete"
                  variant="text"
                  size="small"
                  @click.stop="openDeleteDialog(project)"
                ></v-btn>
              </template>
            </v-list-item>
          </v-list>
        </v-sheet>
      </v-window-item>

      <v-window-item value="create">
        <v-sheet min-width="600" class="mx-auto">
          <v-list-subheader>Create a New Project</v-list-subheader>

          <v-form ref="createFormRef" @submit.prevent="handleCreateProject">
            <v-text-field
              v-model="newProject.name"
              label="Project Name"
              :rules="[rules.required]"
              class="mb-2"
            ></v-text-field>

            <v-textarea
              v-model="newProject.description"
              label="Description (Optional)"
              rows="3"
              class="mb-2"
            ></v-textarea>

            <v-row>
              <v-col cols="8" md="6">
                <v-select
                  v-model="newProject.buildingType"
                  :items="buildingTypeItems"
                  label="Building Type"
                  :rules="[rules.requiredSelect]"
                  class="mb-2"
                ></v-select>
              </v-col>
            </v-row>

            <v-btn
              type="submit"
              color="primary"
              :loading="isCreating"
              block
              class="mt-4"
            >
              Create Project
            </v-btn>

            <v-alert
              v-if="createError"
              type="error"
              :text="createError"
              class="mt-4"
            ></v-alert>
            <v-alert
              v-if="createSuccess"
              type="success"
              :text="createSuccess"
              class="mt-4"
            ></v-alert>
          </v-form>
        </v-sheet>
      </v-window-item>
    </v-window>

    <v-dialog v-model="isUpdateDialogVisible" max-width="800" persistent>
      <v-card>
        <v-card-title class="pa-4">
          <span class="text-h5">Edit Project</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="updateFormRef" @submit.prevent="handleUpdateProject">
            <v-text-field
              v-model="editingProject.name"
              label="Project Name"
              :rules="[rules.required]"
              class="mb-2"
            ></v-text-field>

            <v-textarea
              v-model="editingProject.description"
              label="Description (Optional)"
              rows="3"
              class="mb-2"
            ></v-textarea>

            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="editingProject.buildingType"
                  :items="buildingTypeItems"
                  label="Building Type"
                  :rules="[rules.requiredSelect]"
                  class="mb-2"
                ></v-select>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="isUpdateDialogVisible = false"
            >Cancel</v-btn
          >
          <v-btn
            color="success"
            variant="flat"
            :loading="isUpdating"
            @click="handleUpdateProject"
            >Save Changes</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="isDeleteDialogVisible" max-width="500" persistent>
      <v-card>
        <v-card-title class="text-h5">Delete Project?</v-card-title>
        <v-card-text>
          Are you sure you want to delete
          <strong>{{
            deletingProject ? deletingProject.name : "this project"
          }}</strong
          >? This action cannot be undone.
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="isDeleteDialogVisible = false"
            >Cancel</v-btn
          >
          <v-btn
            color="red-darken-1"
            variant="flat"
            :loading="isDeleting"
            @click="handleDeleteProject"
            >Delete</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="3000">
      {{ snackbarText }}
      <template v-slot:actions>
        <v-btn color="white" variant="text" @click="snackbar = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useTheme } from "vuetify";
import { useRouter } from "vue-router";
const router = useRouter();

// api url
const CORS_PROXY_URL = "https://corsproxy.io/?";
const API_BASE_URL = "http://planstack.dk/api";
const PROJECTS_API_URL = `${API_BASE_URL}/projects`;

const tab = ref("select");

const projects = ref([]);
const isLoadingProjects = ref(false);
const projectsError = ref(null);
const createFormRef = ref(null);
const isCreating = ref(false);
const createError = ref(null);
const createSuccess = ref(null);
const newProject = ref({
  name: "",
  description: "",
  buildingType: null,
});
const isUpdateDialogVisible = ref(false);
const isUpdating = ref(false);
const updateFormRef = ref(null);
const editingProject = ref({
  id: null,
  name: "",
  description: "",
  buildingType: null,
});
const isDeleteDialogVisible = ref(false);
const isDeleting = ref(false);
const deletingProject = ref(null);

// snackbar states
const snackbar = ref(false);
const snackbarText = ref("");
const snackbarColor = ref("success");

// select for buildingtypes
const buildingTypeItems = ref([
  { title: "House", value: 0 },
  { title: "Mansion", value: 1 },
  { title: "Apartment", value: 2 },
  { title: "Office", value: 3 },
  { title: "Parking lot", value: 4 },
  { title: "Park", value: 5 },
  { title: "Other", value: 99 },
]);

const rules = {
  required: (v) => !!v || "This field is required",
  requiredSelect: (v) => v !== null || "This field is required",
  positiveNumber: (v) => (v && v > 0) || "Value must be a positive number",
};

const showSnackbar = (text, color = "success") => {
  snackbarText.value = text;
  snackbarColor.value = color;
  snackbar.value = true;
};

const fetchProjects = async () => {
  isLoadingProjects.value = true;
  projectsError.value = null;
  projects.value = [];

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(PROJECTS_API_URL)}`;
    const response = await fetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      throw new Error(`API returned error: Status ${response.status}`);
    }
    const data = await response.json();
    projects.value = data.entities || [];
  } catch (error) {
    console.error("Error fetching projects:", error);
    projectsError.value = `Failed to load projects: ${error.message}`;
  } finally {
    isLoadingProjects.value = false;
  }
};
//router to floorplanner for selected project
const selectProject = (project) => {
  router.push(`/project/${project.id}/floorplans`);
};

const handleCreateProject = async () => {
  createError.value = null;
  createSuccess.value = null;

  // 1. Validate form
  const { valid } = await createFormRef.value.validate();
  if (!valid) return;

  //   // will be used when userid is working
  //   const userId = getUserId();
  //   if (!userId) {
  //     createError.value =
  //       "You are not logged in. Please login to create a project.";
  //     return;
  //   }

  isCreating.value = true;

  const payload = {
    ...newProject.value,
    // userId: userId, // when userid is ready
  };

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(PROJECTS_API_URL)}`;
    const response = await fetch(proxiedUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(payload),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "An unknown error occurred");
    }

    createSuccess.value = "Project created successfully!";
    newProject.value = {
      name: "",
      description: "",
      buildingType: null,
    };
    await fetchProjects();
    tab.value = "select";
  } catch (error) {
    console.error("Error creating project:", error);
    createError.value = `Failed to create project: ${error.message}`;
  } finally {
    isCreating.value = false;
  }
};

//opens an fills fields with project
const openUpdateDialog = (project) => {
  editingProject.value = Object.assign({}, project);
  isUpdateDialogVisible.value = true;
};

const handleUpdateProject = async () => {
  const { valid } = await updateFormRef.value.validate();
  if (!valid) return;

  //   const userId = getUserId();
  //   if (!userId) {
  //     showSnackbar("You are not logged in.", "error");
  //     return;
  //   }

  isUpdating.value = true;

  const payload = {
    ...editingProject.value,
    // userId: userId, //when userid is working
  };

  try {
    const url = `${PROJECTS_API_URL}/${editingProject.value.id}`;
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await fetch(proxiedUrl, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.message || "Failed to update project");
    }

    isUpdateDialogVisible.value = false;
    showSnackbar("Project updated successfully!");
    await fetchProjects();
  } catch (error) {
    console.error("Error updating project:", error);
    showSnackbar(`Error: ${error.message}`, "error");
  } finally {
    isUpdating.value = false;
  }
};

const openDeleteDialog = (project) => {
  deletingProject.value = project;
  isDeleteDialogVisible.value = true;
};

const handleDeleteProject = async () => {
  if (!deletingProject.value) return;

  isDeleting.value = true;

  try {
    const url = `${PROJECTS_API_URL}/${deletingProject.value.id}`;
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await fetch(proxiedUrl, {
      method: "DELETE",
      headers: {
        Host: "planstack.dk",
      },
    });

    if (!response.ok) {
      if (response.status === 404) throw new Error("project not found.");
      throw new Error(`API error: Status ${response.status}`);
    }

    isDeleteDialogVisible.value = false;
    showSnackbar("Project deleted successfully!");
    await fetchProjects();
  } catch (error) {
    console.error("Error deleting project:", error);
    showSnackbar(`Error: ${error.message}`, "error");
  } finally {
    isDeleting.value = false;
    deletingProject.value = null;
  }
};

//for showing type in list
const getBuildingType = (value) => {
  const item = buildingTypeItems.value.find((item) => item.value === value);
  return item ? item.title : "unknown type";
};

onMounted(() => {
  fetchProjects();
});
</script>
