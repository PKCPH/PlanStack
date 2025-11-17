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
// api
const API_BASE_URL = "http://planstack.dk/api";
const RULESETS_API_URL = `${API_BASE_URL}/rulesets`;

const emptyRuleModel = ref({
  definition: null,
  comparison: null,
  objectTypeDefinition: null,
  objectTypeComparison: null,
});

// RuleSetDefinitionEnum
const definitionItems = ref([
  { title: "By Distance", value: 0 },
  { title: "Blueprint Area Over Ratio", value: 1 },
  { title: "Blueprint Area Exact Ratio", value: 2 },
  { title: "Blueprint Area Under Ratio", value: 3 },
  { title: "Room Area Under Ratio", value: 4 },
  { title: "Room Area Exact Ratio", value: 10 },
  { title: "Room Area Over Ratio", value: 5 },
  { title: "Total Quantity Under Ratio", value: 6 },
  { title: "Total Quantity Over Ratio", value: 7 },
  { title: "Occupancy Over Ratio", value: 8 },
  { title: "Occupancy Under Ratio", value: 9 },
  { title: "Other", value: 99 },
]);
// RuleSetComparisonEnum
const comparisonItems = ref([
  { title: "Minimum", value: 0 },
  { title: "Maximum", value: 1 },
  { title: "Exact", value: 2 },
]);

// RuleSetObjectTypeEnum
const objectTypeItems = ref([
  { title: "Bathroom", value: 0 },
  { title: "Bedroom", value: 1 },
  { title: "Living Room", value: 2 },
  { title: "Kitchen", value: 3 },
  { title: "Dining Room", value: 4 },
  { title: "Wall", value: 20 },
  { title: "Window", value: 21 },
  { title: "Door", value: 22 },
  { title: "Toilet", value: 41 },
  { title: "Fire Safety Equipment", value: 42 },
  { title: "Fridge", value: 43 },
  { title: "Stove", value: 44 },
  { title: "Sink", value: 45 },
  { title: "Kitchen Counter", value: 46 },
  { title: "Couch", value: 47 },
  { title: "Table", value: 48 },
  { title: "Chair", value: 49 },
  { title: "Bed", value: 50 },
  { title: "Closet", value: 51 },
  { title: "Room", value: 71 },
  { title: "Blueprint", value: 72 },
  { title: "Component", value: 73 },
  { title: "Other", value: 99 },
]);

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
