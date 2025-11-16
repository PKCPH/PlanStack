<template>
  <v-container>
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4">Component Library</h2>
      </v-col>
      <v-col class="text-right">
        <v-btn color="primary" @click="openDialog">
          <v-icon start>mdi-plus</v-icon>
          Add New Component
        </v-btn>
      </v-col>
    </v-row>

    <!-- component table -->
    <v-card>
      <v-toolbar flat density="compact">
        <v-spacer></v-spacer>
        <v-text-field
          v-model="searchQuery"
          label="Search Components"
          prepend-inner-icon="mdi-magnify"
          density="compact"
          variant="solo"
          hide-details
          single-line
          clearable
          style="max-width: 300px"
        ></v-text-field>
      </v-toolbar>
      <v-divider></v-divider>

      <v-data-table
        :headers="tableHeaders"
        :items="componentsList"
        :loading="isLoadingList"
        :items-per-page="10"
        :search="searchQuery"
        class="elevation-1"
      >
        <!-- loading -->
        <template v-slot:loading>
          <v-skeleton-loader type="table-row@5"></v-skeleton-loader>
        </template>

        <!-- errors-->
        <template v-slot:no-data>
          <v-alert
            :type="listError ? 'error' : 'info'"
            variant="tonal"
            class="ma-4"
          >
            {{
              listError
                ? listError
                : 'No components found. Click "Add New Component" to get started.'
            }}
          </v-alert>
        </template>

        <!-- image -->
        <template v-slot:item.imgPath="{ item }">
          <v-avatar class="ma-2" rounded="0">
            <v-img
              :src="transformImageUrl(item.imgPath)"
              :alt="item.name"
              height="40"
              width="40"
              cover
            >
              <template v-slot:error>
                <v-icon color="grey-lighten-1">mdi-image-off</v-icon>
              </template>
            </v-img>
          </v-avatar>
        </template>

        <!-- categories -->
        <template v-slot:item.category="{ item }">
          <v-chip size="small" :color="getCategoryColor(item.category)">
            {{ getCategoryName(item.category) }}
          </v-chip>
        </template>

        <!-- prices -->
        <template v-slot:item.price="{ item }">
          <span class="text-green-darken-1 font-weight-bold">
            ${{ item.price ? item.price.toFixed(2) : "0.00" }}
          </span>
        </template>

        <!-- size -->
        <template v-slot:item.squareMeters="{ item }">
          <span>{{ item.squareMeters }} m²</span>
        </template>

        <template v-slot:item.height="{ item }">
          <span>{{ item.height || 0 }} m</span>
        </template>

        <template v-slot:item.width="{ item }">
          <span>{{ item.width || 0 }} m</span>
        </template>

        <!-- table actions -->
        <template v-slot:item.actions="{ item }">
          <v-icon
            small
            class="me-2"
            @click="editComponent(item)"
            color="grey-darken-1"
            aria-label="Edit item"
          >
            mdi-pencil
          </v-icon>
          <v-icon
            small
            @click="deleteComponent(item)"
            color="red-darken-1"
            aria-label="Delete item"
          >
            mdi-delete
          </v-icon>
        </template>
      </v-data-table>
    </v-card>

    <!-- dialog for create and update -->
    <v-dialog v-model="dialog" max-width="800px" persistent>
      <v-card>
        <v-card-title>
          <span class="text-h5">{{
            editingId ? "Edit Component" : "Add New Component"
          }}</span>
        </v-card-title>

        <v-card-text>
          <v-form ref="formRef">
            <v-container>
              <v-text-field
                v-model="formData.name"
                label="Name"
                :rules="rules.required"
                density="compact"
                class="mb-2"
              ></v-text-field>

              <v-textarea
                v-model="formData.description"
                label="Description"
                rows="2"
                density="compact"
                class="mb-2"
              ></v-textarea>

              <v-row>
                <v-col cols="12" md="6">
                  <v-select
                    v-model="formData.category"
                    :items="categoryOptions"
                    label="Category"
                    :rules="rules.required"
                    density="compact"
                  ></v-select>
                </v-col>

                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="formData.model"
                    label="Model/SKU"
                    density="compact"
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model.number="formData.price"
                    label="Price"
                    type="number"
                    min="0"
                    prefix="$"
                    density="compact"
                  ></v-text-field>
                </v-col>

                <v-col cols="12" md="6">
                  <v-text-field
                    v-model.number="formData.squareMeters"
                    label="Square Meters"
                    type="number"
                    min="0"
                    suffix="m²"
                    density="compact"
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model.number="formData.height"
                    label="Height"
                    type="number"
                    min="0"
                    suffix="m"
                    density="compact"
                  ></v-text-field>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model.number="formData.width"
                    label="Width"
                    type="number"
                    min="0"
                    suffix="m"
                    density="compact"
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-file-input
                v-model="formData.imgFile"
                label="Component Image"
                accept="image/*"
                :rules="editingId ? [] : rules.requiredFile"
                :hint="
                  editingId
                    ? 'Leave this field blank to keep existing image'
                    : ''
                "
                persistent-hint
                density="compact"
                class="mt-2"
                show-size
              ></v-file-input>
            </v-container>
          </v-form>

          <v-alert v-if="saveError" type="error" density="compact" class="mt-4">
            {{ saveError }}
          </v-alert>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="closeDialog"
            :disabled="isSaving"
          >
            Cancel
          </v-btn>
          <v-btn
            color="blue-darken-1"
            variant="elevated"
            @click="handleSave"
            :loading="isSaving"
          >
            Save
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- delete dialog -->
    <v-dialog v-model="isDeletingDialog" max-width="500px" persistent>
      <v-card>
        <v-card-title class="text-h5"> Are you sure? </v-card-title>
        <v-card-text>
          Are you sure you want to delete the component
          <strong>{{ componentToDelete?.name || "this item" }}</strong
          >? This action cannot be undone.
        </v-card-text>

        <v-alert v-if="deleteError" type="error" density="compact" class="mx-4">
          {{ deleteError }}
        </v-alert>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="closeDeleteDialog"
            :disabled="isDeleting"
          >
            Cancel
          </v-btn>
          <v-btn
            color="red-darken-1"
            variant="elevated"
            @click="confirmDelete"
            :loading="isDeleting"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
// import { apiFetch } from '../components/api/auth.js';

const CORS_PROXY_URL = "https://corsproxy.io/?";
const API_BASE_URL = "http://planstack.dk";
const COMPONENTS_API_URL = `${API_BASE_URL}/api/components`;

const dialog = ref(false);
const formRef = ref(null);
const editingId = ref(null);

const defaultFormData = {
  name: "",
  description: "",
  category: null,
  model: "",
  price: 0,
  squareMeters: 0,
  height: 0,
  width: 0,
  imgFile: null,
};
const formData = ref({ ...defaultFormData });

// saving
const isSaving = ref(false);
const saveError = ref(null);

//table
const componentsList = ref([]);
const isLoadingList = ref(false);
const listError = ref(null);
const searchQuery = ref("");

// delete
const isDeletingDialog = ref(false);
const isDeleting = ref(false);
const componentToDelete = ref(null);
const deleteError = ref(null);

const tableHeaders = ref([
  {
    title: "Image",
    key: "imgPath",
    sortable: false,
    width: "80px",
    align: "center",
  },
  { title: "Name", key: "name" },
  { title: "Category", key: "category" },
  { title: "Model/SKU", key: "model", sortable: false },
  { title: "Price", key: "price" },
  { title: "Size", key: "squareMeters" },
  { title: "Height", key: "height" },
  { title: "Width", key: "width" },
  { title: "Actions", key: "actions", sortable: false, align: "end" },
]);

const categoryOptions = [
  { title: "Table", value: 0, color: "blue-lighten-1" },
  { title: "Toilet", value: 1, color: "teal-lighten-1" },
  { title: "Chair", value: 2, color: "orange-lighten-1" },
  { title: "Sofa", value: 3, color: "red-lighten-1" },
  { title: "Cabinet", value: 4, color: "purple-lighten-1" },
  { title: "Wardrobe", value: 5, color: "brown-lighten-1" },
  { title: "Fire Safety Equipment", value: 6, color: "pink-lighten-1" },
  { title: "Fridge", value: 7, color: "cyan-lighten-1" },
  { title: "Stove", value: 8, color: "lime-lighten-1" },
  { title: "Sink", value: 9, color: "amber-lighten-1" },
  { title: "Kitchen Counter", value: 10, color: "indigo-lighten-1" },
  { title: "Bed", value: 11, color: "green-lighten-1" },
  { title: "Other", value: 99, color: "grey-lighten-1" },
];

// validation
const rules = {
  required: [(v) => !!v || "This field is required"],
  requiredFile: [(v) => !!v || "A file is required"],
};

const openDialog = () => {
  editingId.value = null;
  resetForm();
  dialog.value = true;
};

const closeDialog = () => {
  dialog.value = false;
  resetForm();
};

const resetForm = () => {
  formData.value = { ...defaultFormData };
  saveError.value = null;
  editingId.value = null;
  formRef.value?.resetValidation();
};

// functions for comp list

const fetchComponents = async () => {
  isLoadingList.value = true;
  listError.value = null;
  componentsList.value = [];

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(COMPONENTS_API_URL)}`;
    const response = await fetch(proxiedUrl, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    const data = await response.json();

    if (data && data.entities) {
      componentsList.value = data.entities;
    } else if (Array.isArray(data)) {
      componentsList.value = data;
    } else {
      console.warn("Unexpected API response format:", data);
    }
  } catch (error) {
    console.error("Error fetching components:", error);
    listError.value = error.message;
  } finally {
    isLoadingList.value = false;
  }
};

const transformImageUrl = (imgPath) => {
  if (!imgPath) {
    return "";
  }
  const fileName = imgPath.split(/[\\\/]/).pop();
  const webUrl = `${API_BASE_URL}/api/Uploads/${fileName}`;
  return `${CORS_PROXY_URL}${encodeURIComponent(webUrl)}`;
};

const getCategoryName = (value) => {
  const category = categoryOptions.find((c) => c.value === value);
  return category ? category.title : "Unknown";
};

const getCategoryColor = (value) => {
  const category = categoryOptions.find((c) => c.value === value);
  return category ? category.color : "grey";
};

const editComponent = (component) => {
  editingId.value = component.id;
  formData.value = {
    name: component.name,
    description: component.description,
    category: component.category,
    model: component.model,
    price: component.price,
    squareMeters: component.squareMeters,
    height: component.height || 0,
    width: component.width || 0,
    imgFile: null,
  };
  dialog.value = true;
};

// delete funcs

const deleteComponent = (component) => {
  componentToDelete.value = component;
  deleteError.value = null;
  isDeletingDialog.value = true;
};

const closeDeleteDialog = () => {
  isDeletingDialog.value = false;
  isDeleting.value = false;
  deleteError.value = null;
  componentToDelete.value = null;
};

const confirmDelete = async () => {
  if (!componentToDelete.value) return;

  isDeleting.value = true;
  deleteError.value = null;

  const id = componentToDelete.value.id;
  const url = `${COMPONENTS_API_URL}/${id}`;

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await fetch(proxiedUrl, {
      method: "DELETE",
      headers: { Host: "planstack.dk" },
    });

    if (response.ok) {
      console.log("Component deleted successfully");
      closeDeleteDialog();
      // to refresh list after deleting
      fetchComponents();
    } else {
      let errorMessage = `API Error: ${response.status} ${response.statusText}`;
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || JSON.stringify(errorData);
      } catch (e) {}
      throw new Error(errorMessage);
    }
  } catch (error) {
    console.error("Error deleting component:", error);
    deleteError.value = error.message; // error to dialog
  } finally {
    isDeleting.value = false;
  }
};

//to handle create and edit
const handleSave = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  isSaving.value = true;
  saveError.value = null;

  const payload = new FormData();
  payload.append("Name", formData.value.name);
  payload.append("Description", formData.value.description);
  payload.append("Category", formData.value.category);
  payload.append("Model", formData.value.model);
  payload.append("Price", formData.value.price);
  payload.append("SquareMeters", formData.value.squareMeters);
  payload.append("Height", formData.value.height);
  payload.append("Width", formData.value.width);

  if (formData.value.imgFile) {
    payload.append("ImgFile", formData.value.imgFile);
  }

  const isUpdating = !!editingId.value;
  const method = isUpdating ? "PUT" : "POST";
  const url = isUpdating
    ? `${COMPONENTS_API_URL}/${editingId.value}`
    : COMPONENTS_API_URL;

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(url)}`;

    const response = await fetch(proxiedUrl, {
      method: method,
      headers: { Host: "planstack.dk" },
      body: payload,
    });

    if (response.ok) {
      let data = null;
      if (response.status !== 204) {
        data = await response.json();
        console.log("Component saved successfully", data);
      } else {
        console.log("Component updated successfully");
      }

      closeDialog();
      fetchComponents();
    } else {
      let errorMessage = `Error: ${response.status} ${response.statusText}`;
      try {
        const errorData = await response.json();
        if (errorData.title) {
          errorMessage = errorData.title;
        } else if (errorData.message) {
          errorMessage = errorData.message;
        } else {
          errorMessage = JSON.stringify(errorData);
        }
      } catch (e) {}
      throw new Error(errorMessage);
    }
  } catch (error) {
    console.error("Error saving component:", error);
    saveError.value = error.message;
  } finally {
    isSaving.value = false;
  }
};

onMounted(() => {
  fetchComponents();
});
</script>
