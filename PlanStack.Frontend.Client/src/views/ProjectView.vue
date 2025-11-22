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
import { API_CONFIG } from "@/components/api/config.js";
import buildingTypeItems from "@/assets/enums/buildingTypeOptions.json";

const router = useRouter();

const PROJECTS_API_URL = API_CONFIG.ENDPOINTS.PROJECTS;

const newProject = ref({
  name: "",
  description: "",
  buildingType: null,
});

//form validation
const rules = {
  required: (v) => !!v || "This field is required",
  requiredSelect: (v) => {
    return (v !== null && v !== undefined) || "This field is required";
  },
};

const getBuildingType = (value) => {
  const item = buildingTypeItems.find((item) => item.value === value);
  return item ? item.title : "unknown type";
};

// for router
const selectProject = (project) => {
  router.push(`/project/${project.id}/floorplans`);
};
</script>
