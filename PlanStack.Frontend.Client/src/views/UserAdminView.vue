<template>
  <v-container class="py-10">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">User Admin</h1>
    </div>

    <v-sheet max-width="900" class="mx-auto" rounded="lg" border>
      <v-data-table
        :headers="headers"
        :items="users"
        :loading="isLoading"
        loading-text="Loading users..."
        class="elevation-0"
      >
        <!-- standards count -->
        <template v-slot:item.standards="{ item }">
          <span>{{
            Array.isArray(item.standards) ? item.standards.length : 0
          }}</span>
        </template>

        <!-- projects count -->
        <template v-slot:item.projects="{ item }">
          <span>{{
            Array.isArray(item.projects) ? item.projects.length : 0
          }}</span>
        </template>

        <!-- actions -->
        <template v-slot:item.actions="{ item }">
          <v-btn
            color="red-lighten-1"
            icon="mdi-delete"
            variant="text"
            size="small"
            @click.stop="openDeleteDialog(item)"
            title="Delete User"
          ></v-btn>
        </template>

        <!-- if error -->
        <template v-slot:no-data>
          <v-alert
            v-if="error"
            type="error"
            title="Error loading users"
            :text="error"
            class="ma-4"
          ></v-alert>
          <v-alert
            v-else
            type="info"
            text="No users found."
            class="ma-4"
          ></v-alert>
        </template>
      </v-data-table>
    </v-sheet>

    <!-- delete dialog-->
    <v-dialog v-model="isDeleteDialogVisible" max-width="500" persistent>
      <v-card>
        <v-card-title class="text-h5">Delete User?</v-card-title>
        <v-card-text>
          Are you sure you want to delete
          <strong>{{ itemToDelete ? itemToDelete.email : "this user" }}</strong
          >? This action cannot be undone.
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="grey-darken-1" variant="text" @click="closeDeleteDialog"
            >Cancel</v-btn
          >
          <v-btn
            color="red-darken-1"
            variant="flat"
            :loading="isDeleting"
            @click="handleDeleteUser"
            >Delete</v-btn
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
import { ref, onMounted } from "vue";
import { apiFetch } from "../components/api/auth.js";
import { API_CONFIG } from "../components/api/config.js";

const USERS_API_URL = API_CONFIG.ENDPOINTS.USERS;
// table
const users = ref([]);
const isLoading = ref(false);
const error = ref(null);
const headers = ref([
  { title: "Email", key: "email" },
  { title: "User Name", key: "userName" },
  { title: "Standards", key: "standards" },
  { title: "Projects", key: "projects" },
  { title: "Actions", key: "actions", sortable: false, align: "end" },
]);

// delete diolog
const isDeleteDialogVisible = ref(false);
const isDeleting = ref(false);
const itemToDelete = ref(null);

// snackbar
const snackbar = ref(false);
const snackbarText = ref("");
const snackbarColor = ref("success");

const showSnackbar = (text, color = "success") => {
  snackbarText.value = text;
  snackbarColor.value = color;
  snackbar.value = true;
};

//alll users
const fetchUsers = async () => {
  isLoading.value = true;
  error.value = null;
  users.value = [];

  try {
    const result = await apiFetch(USERS_API_URL, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!result.ok) {
      throw new Error(`API returned error: Status ${result.status}`);
    }

    users.value = result.data || [];
    console.log("Fetched users:", users.value);
  } catch (e) {
    console.error("Error fetching users:", e);
    error.value = `Failed to load users: ${e.message}`;
  } finally {
    isLoading.value = false;
  }
};

const openDeleteDialog = (item) => {
  itemToDelete.value = item;
  isDeleteDialogVisible.value = true;
};

const closeDeleteDialog = () => {
  isDeleteDialogVisible.value = false;
  itemToDelete.value = null;
};

const handleDeleteUser = async () => {
  if (!itemToDelete.value) return;

  isDeleting.value = true;

  try {
    const url = `${USERS_API_URL}/${itemToDelete.value.email}`;

    const response = await apiFetch(url, {
      method: "DELETE",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      if (response.status === 404) throw new Error("User not found.");
      throw new Error(`API error: Status ${response.status}`);
    }

    showSnackbar("User deleted successfully!");
    closeDeleteDialog();
    await fetchUsers(); //refresh
  } catch (e) {
    console.error("Error deleting user:", e);
    showSnackbar(`Error: ${e.message}`, "error");
  } finally {
    isDeleting.value = false;
  }
};

onMounted(() => {
  fetchUsers();
});
</script>
