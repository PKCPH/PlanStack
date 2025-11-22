<template>
  <v-container>
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4">Building Structures</h2>
      </v-col>
      <v-col class="text-right">
        <v-btn color="primary" @click="openDialog">
          <v-icon start>mdi-plus</v-icon>
          Add New Buidling Structure
        </v-btn>
      </v-col>
    </v-row>

    <!-- building structure table -->
    <v-card>
      <v-toolbar flat density="compact">
        <v-spacer></v-spacer>
        <v-text-field
          v-model="searchQuery"
          label="Search Building Structures"
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
        :items="structureList"
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
                : 'No structures found. Click "Add New Structure" to get started.'
            }}
          </v-alert>
        </template>

        <template v-slot:item.description="{ item }">
          <span class="d-inline-block text-truncate" style="max-width: 300px">
            {{ item.description || "" }}
          </span>
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

        <!-- table actions -->
        <template v-slot:item.actions="{ item }">
          <v-icon
            small
            class="me-2"
            @click="editItem(item)"
            color="grey-darken-1"
            aria-label="Edit item"
          >
            mdi-pencil
          </v-icon>
          <v-icon
            small
            @click="deleteItem(item)"
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
            editingId ? "Edit Structure" : "Add New Building Structure"
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
                    :items="categoryOptions.filter((c, i) => i !== 0)"
                    label="Category"
                    :rules="[
                      (v) =>
                        v !== null && v !== undefined
                          ? true
                          : 'This field is required',
                    ]"
                    density="compact"
                  ></v-select>
                </v-col>

                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="formData.material"
                    label="Material"
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
              </v-row>
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
          Are you sure you want to delete the structure
          <strong>{{ itemToDelete?.name || "this item" }}</strong
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
import { apiFetch } from "../components/api/auth.js";
import categoryOptions from "../assets/enums/buildingStructureCategoryOptions.json";
import { API_CONFIG } from "../components/api/config.js";

const STRUCTURES_API_URL = API_CONFIG.ENDPOINTS.BUILDING_STRUCTURES;

const dialog = ref(false);
const formRef = ref(null);
const editingId = ref(null);

const defaultFormData = {
  name: "",
  description: "",
  category: null,
  material: "",
  price: 0,
};
const formData = ref({ ...defaultFormData });

// saving
const isSaving = ref(false);
const saveError = ref(null);

// table
const structureList = ref([]);
const isLoadingList = ref(false);
const listError = ref(null);
const searchQuery = ref("");

// delete
const isDeletingDialog = ref(false);
const isDeleting = ref(false);
const itemToDelete = ref(null);
const deleteError = ref(null);

const tableHeaders = ref([
  { title: "Name", key: "name" },
  { title: "Description", key: "description", sortable: false, width: "30%" },
  { title: "Category", key: "category" },
  { title: "Material", key: "material", sortable: false },
  { title: "Price", key: "price" },
  { title: "Actions", key: "actions", sortable: false, align: "end" },
]);

// validation
const rules = {
  required: [(v) => !!v || "This field is required"],
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

const fetchStructures = async () => {
  isLoadingList.value = true;
  listError.value = null;
  structureList.value = [];

  try {
    const response = await apiFetch(STRUCTURES_API_URL, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    const data = await response.json();

    if (data && data.entities) {
      structureList.value = data.entities;
    } else if (Array.isArray(data)) {
      structureList.value = data;
    } else {
      console.warn("Unexpected API response format:", data);
    }
  } catch (error) {
    console.error("Error fetching structures:", error);
    listError.value = error.message;
  } finally {
    isLoadingList.value = false;
  }
};

const getCategoryName = (value) => {
  const category = categoryOptions.find((c, i) => i !== 0 && c.value === value);
  return category ? category.title : "Unknown";
};

const getCategoryColor = (value) => {
  const category = categoryOptions.find((c, i) => i !== 0 && c.value === value);
  return category ? category.color : "grey";
};

const editItem = (item) => {
  editingId.value = item.id;
  formData.value = {
    name: item.name,
    description: item.description,
    category: item.category,
    material: item.material,
    price: item.price,
  };
  dialog.value = true;
};

// delete

const deleteItem = (item) => {
  itemToDelete.value = item;
  deleteError.value = null;
  isDeletingDialog.value = true;
};

const closeDeleteDialog = () => {
  isDeletingDialog.value = false;
  isDeleting.value = false;
  deleteError.value = null;
  itemToDelete.value = null;
};

const confirmDelete = async () => {
  if (!itemToDelete.value) return;

  isDeleting.value = true;
  deleteError.value = null;

  const id = itemToDelete.value.id;
  const url = `${STRUCTURES_API_URL}/${id}`;

  try {
    const response = await apiFetch(url, {
      method: "DELETE",
      headers: { Host: "planstack.dk" },
    });

    if (response.ok) {
      console.log("Structure deleted successfully");
      closeDeleteDialog();
      // to refresh list after deleting
      fetchStructures();
    } else {
      let errorMessage = `API Error: ${response.status} ${response.statusText}`;
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || JSON.stringify(errorData);
      } catch (e) {}
      throw new Error(errorMessage);
    }
  } catch (error) {
    console.error("Error deleting structure:", error);
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

  const payload = {
    name: formData.value.name,
    description: formData.value.description,
    category: formData.value.category,
    material: formData.value.material,
    price: formData.value.price,
  };

  // Note: Your C# code is likely expecting 'id' in the payload for PUT
  if (editingId.value) {
    payload.id = editingId.value;
  }

  const isUpdating = !!editingId.value;
  const method = isUpdating ? "PUT" : "POST";
  const url = isUpdating
    ? `${STRUCTURES_API_URL}/${editingId.value}`
    : STRUCTURES_API_URL;

  try {
    const response = await apiFetch(url, {
      method: method,
      headers: {
        Host: "planstack.dk",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (response.ok) {
      let data = null;
      if (response.status !== 204) {
        data = await response.json();
        console.log("Structure saved successfully", data);
      } else {
        console.log("Structure updated successfully (204 No Content)");
      }

      closeDialog();
      fetchStructures();
    } else {
      let errorMessage = `API Error: ${response.status} ${response.statusText}`;
      try {
        // asp.net format
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
    console.error("Error saving structure:", error);
    saveError.value = error.message;
  } finally {
    isSaving.value = false;
  }
};

onMounted(() => {
  fetchStructures();
});
</script>
