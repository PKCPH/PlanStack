<template>
  <ResourceManager
    title="Project"
    :api-endpoint="PROJECTS_API_URL"
    :empty-state-model="newProject"
    @select="selectProject"
  >
    <template #list-item="{ item }">
      <v-list-item-title>{{ item.name }}</v-list-item-title>
      <v-list-item-subtitle>{{
        item.description || "No description"
      }}</v-list-item-subtitle>
      <v-list-item-subtitle class="mt-2">
        Type: <strong>{{ getBuildingType(item.buildingType) }}</strong>
      </v-list-item-subtitle>
    </template>

    <template #form-fields="{ model }">
      <v-text-field
        v-model="model.name"
        label="Project Name"
        :rules="[rules.required]"
        class="mb-2"
      ></v-text-field>
      <v-textarea
        v-model="model.description"
        label="Description (Optional)"
        rows="3"
        class="mb-2"
      ></v-textarea>
      <v-select
        v-model="model.buildingType"
        :items="buildingTypeItems"
        label="Building Type"
        :rules="[rules.requiredSelect]"
        class="mb-2"
      ></v-select>
    </template>
  </ResourceManager>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import ResourceManager from "@/components/resource/ResourceManager.vue";

const router = useRouter();

const API_BASE_URL = "http://planstack.dk/api";
const PROJECTS_API_URL = `${API_BASE_URL}/projects`;

const newProject = ref({
  name: "",
  description: "",
  buildingType: null,
});

const buildingTypeItems = ref([
  { title: "House", value: 0 },
  { title: "Mansion", value: 1 },
  { title: "Apartment", value: 2 },
  { title: "Office", value: 3 },
  { title: "Parking lot", value: 4 },
  { title: "Park", value: 5 },
  { title: "Other", value: 99 },
]);
//form validatino
const rules = {
  required: (v) => !!v || "This field is required",
  requiredSelect: (v) => v !== null || "This field is required",
};

const getBuildingType = (value) => {
  const item = buildingTypeItems.value.find((item) => item.value === value);
  return item ? item.title : "unknown type";
};

// for router
const selectProject = (project) => {
  router.push(`/project/${project.id}/floorplans`);
};
</script>
