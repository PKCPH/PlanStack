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

    <!-- create component -->
    <v-dialog v-model="dialog" max-width="800px" persistent>
      <v-card>
        <v-card-title>
          <span class="text-h5">Add New Component</span>
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

              <v-file-input
                v-model="formData.imgFile"
                label="Component Image"
                accept="image/*"
                :rules="rules.requiredFile"
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
  </v-container>
</template>

<script setup>
import { ref } from "vue";

const CORS_PROXY_URL = "https://corsproxy.io/?";
const COMPONENTS_API_URL = "http://planstack.dk/api/components";

const dialog = ref(false);

const formRef = ref(null);
const defaultFormData = {
  name: "",
  description: "",
  category: null,
  model: "",
  price: 0,
  squareMeters: 0,
  imgFile: [], // v-file-input v-model (array)
};
const formData = ref({ ...defaultFormData });

const isSaving = ref(false);
const saveError = ref(null);

const categoryOptions = [
  { title: "Table", value: 0 },
  { title: "Toilet", value: 1 },
  { title: "Chair", value: 2 },
  { title: "Sofa", value: 3 },
  { title: "Cabinet", value: 4 },
  { title: "Wardrobe", value: 5 },
  { title: "Other", value: 99 },
];

// validation (TODO)
const rules = {};
const openDialog = () => {
  dialog.value = true;
};

const closeDialog = () => {
  dialog.value = false;
  resetForm();
};

const resetForm = () => {
  formData.value = { ...defaultFormData };
  saveError.value = null;
  formRef.value?.resetValidation();
};

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

  if (formData.value.imgFile && formData.value.imgFile.length > 0) {
    const fileToSend = formData.value.imgFile[0];
    console.log("File being added to FormData:", fileToSend);

    payload.append("ImgFile", fileToSend);
  }

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(COMPONENTS_API_URL)}`;

    const response = await fetch(proxiedUrl, {
      method: "POST",
      headers: { Host: "planstack.dk" },
      body: payload,
    });

    const data = await response.json();

    if (data.success === true) {
      console.log("component saved", data.data);
      closeDialog();
      // TODO: Refresh componentslist
    } else {
      throw new Error(data.message || "An unknown server error occurred.");
    }
  } catch (error) {
    console.error("Error saving component:", error);
    saveError.value = error.message;
  } finally {
    isSaving.value = false;
  }
};
</script>
