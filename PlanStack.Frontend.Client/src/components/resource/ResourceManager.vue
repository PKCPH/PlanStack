<template>
  <v-container class="py-10">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">{{ title }}</h1>
    </div>

    <v-sheet max-width="600" class="mx-auto">
      <div class="d-flex justify-space-between align-center mb-2 pa-4">
        <v-list-subheader class="pa-0">Select a {{ title }}</v-list-subheader>
        <v-btn
          color="secondary"
          @click="openEditDialog(null)"
          prepend-icon="mdi-plus"
          size="small"
        >
          Create New
        </v-btn>
      </div>

      <v-divider></v-divider>

      <v-skeleton-loader
        v-if="isLoading"
        type="list-item-two-line@3"
      ></v-skeleton-loader>

      <v-alert
        v-if="error"
        type="error"
        title="Error loading data"
        :text="error"
        class="ma-4"
      ></v-alert>

      <v-alert
        v-if="!isLoading && !error && items.length === 0"
        type="info"
        :text="`No ${title.toLowerCase()} found. You can create one above.`"
        class="ma-4"
      ></v-alert>

      <v-list
        v-if="!isLoading && items.length > 0"
        lines="two"
        style="max-height: 500px; overflow-y: auto"
      >
        <v-list-item
          v-for="item in items"
          :key="item.id"
          @click="$emit('select', item)"
          ripple
        >
          <slot name="list-item" :item="item"></slot>

          <template v-slot:append>
            <v-btn
              color="grey-darken-1"
              icon="mdi-pencil"
              variant="text"
              size="small"
              class="mr-1"
              @click.stop="openEditDialog(item)"
            ></v-btn>

            <v-btn
              color="red-lighten-1"
              icon="mdi-delete"
              variant="text"
              size="small"
              @click.stop="openDeleteDialog(item)"
            ></v-btn>
          </template>
        </v-list-item>
      </v-list>
    </v-sheet>

    <v-dialog v-model="isEditDialogVisible" max-width="800" persistent>
      <v-card>
        <v-card-title class="pa-4">
          <span class="text-h5">{{
            editingItem.id ? `Edit ${title}` : `Create New ${title}`
          }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="formRef">
            <slot name="form-fields" :model="editingItem"></slot>
          </v-form>
        </v-card-text>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="isEditDialogVisible = false"
            >Cancel</v-btn
          >
          <v-btn
            color="success"
            variant="flat"
            :loading="isSaving"
            @click="handleSave"
            >Save</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="isDeleteDialogVisible" max-width="500" persistent>
      <v-card>
        <v-card-title class="text-h5">Delete {{ title }}?</v-card-title>
        <v-card-text>
          Are you sure you want to delete
          <strong>{{ deletingItem ? deletingItem.name : "this item" }}</strong
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
            @click="handleDelete"
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

// modelling props
const props = defineProps({
  title: {
    type: String,
    required: true,
  },
  apiEndpoint: {
    type: String,
    required: true,
  },
  emptyStateModel: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(["select"]);

// corsproxy for api
const CORS_PROXY_URL = "https://corsproxy.io/?";

// component states
const items = ref([]);
const isLoading = ref(false);
const error = ref(null);
const formRef = ref(null);

const isEditDialogVisible = ref(false);
const isSaving = ref(false);
const editingItem = ref({ ...props.emptyStateModel });

const isDeleteDialogVisible = ref(false);
const isDeleting = ref(false);
const deletingItem = ref(null);

// snackbar states

const snackbar = ref(false);
const snackbarText = ref("");
const snackbarColor = ref("success");

const showSnackbar = (text, color = "success") => {
  snackbarText.value = text;
  snackbarColor.value = color;
  snackbar.value = true;
};

const fetchItems = async () => {
  isLoading.value = true;
  error.value = null;
  items.value = [];

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(
      props.apiEndpoint
    )}`;
    const response = await fetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      throw new Error(`API returned error: Status ${response.status}`);
    }
    const data = await response.json();
    items.value = data.entities || [];
  } catch (e) {
    console.error("Error fetching items:", e);
    error.value = `Failed to load data: ${e.message}`;
  } finally {
    isLoading.value = false;
  }
};

const openEditDialog = (item) => {
  if (item) {
    // edit mode
    editingItem.value = Object.assign({}, item);
  } else {
    // create mode
    editingItem.value = { ...props.emptyStateModel };
  }
  isEditDialogVisible.value = true;
};

const openDeleteDialog = (item) => {
  deletingItem.value = item;
  isDeleteDialogVisible.value = true;
};

const handleSave = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  isSaving.value = true;
  const isUpdating = !!editingItem.value.id;

  const method = isUpdating ? "PUT" : "POST";
  const url = isUpdating
    ? `${props.apiEndpoint}/${editingItem.value.id}`
    : props.apiEndpoint;

  const payload = { ...editingItem.value };
  // Add auth/user ID if needed
  // payload.userId = getUserId();

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;
    const response = await fetch(proxiedUrl, {
      method: method,
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.message || "An unknown error occurred");
    }

    showSnackbar(
      `${props.title} ${isUpdating ? "updated" : "created"} successfully!`
    );
    isEditDialogVisible.value = false;
    await fetchItems();
  } catch (e) {
    console.error("Error saving item:", e);
    showSnackbar(`Error: ${e.message}`, "error");
  } finally {
    isSaving.value = false;
  }
};

const handleDelete = async () => {
  if (!deletingItem.value) return;

  isDeleting.value = true;

  try {
    const url = `${props.apiEndpoint}/${deletingItem.value.id}`;
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await fetch(proxiedUrl, {
      method: "DELETE",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      if (response.status === 404) throw new Error("Item not found.");
      throw new Error(`API error: Status ${response.status}`);
    }

    showSnackbar(`${props.title} deleted successfully!`);
    isDeleteDialogVisible.value = false;
    await fetchItems();
  } catch (e) {
    console.error("Error deleting item:", e);
    showSnackbar(`Error: ${e.message}`, "error");
  } finally {
    isDeleting.value = false;
    deletingItem.value = null;
  }
};

onMounted(() => {
  fetchItems();
});

// Expose fetchItems to parent if needed
defineExpose({ fetchItems });
</script>
