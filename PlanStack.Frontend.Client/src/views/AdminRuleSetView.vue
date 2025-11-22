<template>
  <ResourceManager
    title="Base Rulesets"
    :api-endpoint="RULESETS_API_URL"
    :empty-state-model="emptyRuleModel"
  >
    <template #list-item="{ item }">
      <v-list-item-title class="font-weight-medium">
        {{ getDefinitionName(item.definition) }}
        {{ getComparisonName(item.comparison) }}
      </v-list-item-title>
      <v-list-item-subtitle>
        Defines:
        <v-chip size="small" class="ml-1">{{
          getObjectTypeName(item.objectTypeDefinition)
        }}</v-chip>
      </v-list-item-subtitle>
      <v-list-item-subtitle>
        Compares:
        <v-chip size="small" class="ml-1">{{
          getObjectTypeName(item.objectTypeComparison)
        }}</v-chip>
      </v-list-item-subtitle>
    </template>
    <template #form-fields="{ model }">
      <v-select
        v-model="model.definition"
        :items="definitionItems"
        item-title="title"
        item-value="value"
        label="Definition"
        :rules="[rules.requiredSelect]"
        class="mb-2"
        density="compact"
      ></v-select>

      <v-select
        v-model="model.comparison"
        :items="comparisonItems"
        item-title="title"
        item-value="value"
        label="Comparison"
        :rules="[rules.requiredSelect]"
        class="mb-2"
        density="compact"
      ></v-select>

      <v-select
        v-model="model.objectTypeDefinition"
        :items="objectTypeItems"
        item-title="title"
        item-value="value"
        label="Object Type Definition"
        class="mb-2"
        density="compact"
        clearable
      ></v-select>

      <v-select
        v-model="model.objectTypeComparison"
        :items="objectTypeItems"
        item-title="title"
        item-value="value"
        label="Object Type Comparison"
        class="mb-2"
        density="compact"
        clearable
      ></v-select>
    </template>
  </ResourceManager>
</template>

<script setup>
import { ref } from "vue";
import ResourceManager from "@/components/resource/ResourceManager.vue";
import definitionItems from "@/assets/enums/ruleSetDefinitionOptions.json";
import comparisonItems from "@/assets/enums/ruleSetComparisonOptions.json";
import objectTypeItems from "@/assets/enums/ruleSetObjectTypeOptions.json";
import { API_CONFIG } from "../components/api/config.js";

// api
const RULESETS_API_URL = API_CONFIG.ENDPOINTS.RULESETS;
const emptyRuleModel = ref({
  definition: null,
  comparison: null,
  objectTypeDefinition: null,
  objectTypeComparison: null,
});

const rules = {
  required: (v) => (v !== null && v !== "") || "This field is required",
  requiredSelect: (v) => v !== null || "This field is required",
};

const getDefinitionName = (value) => {
  const item = definitionItems.value.find((item) => item.value === value);
  return item ? item.title : "Not Assigned";
};
const getComparisonName = (value) => {
  const item = comparisonItems.value.find((item) => item.value === value);
  return item ? item.title : "Not Assigned";
};
const getObjectTypeName = (value) => {
  if (value === null || value === undefined) return "Not Assigned";
  const item = objectTypeItems.value.find((item) => item.value === value);
  return item ? item.title : "Not Assigned";
};
</script>
