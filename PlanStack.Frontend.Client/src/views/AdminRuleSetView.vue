<template>
  <ResourceManager
    title="Base Rulesets"
    :api-endpoint="RULESETS_API_URL"
    :empty-state-model="emptyRuleModel"
  >
    <template #list-item="{ item }">
      <v-list-item-title class="font-weight-medium">
        {{ item.name }}
      </v-list-item-title>

      <v-list-item-subtitle class="mt-1">
        <strong>Logic:</strong> Measured
        {{ getObjectTypeName(item.objectTypeDefinition) }}
        {{ getDefinitionName(item.definition) }} with
        {{ getComparisonName(item.comparison) }} amount of
        {{ getObjectTypeName(item.objectTypeComparison) }}
      </v-list-item-subtitle>

      <v-list-item-subtitle class="mt-1">
        <v-chip
          size="x-small"
          label
          class="mr-1"
          v-if="item.objectTypeDefinition"
        >
          DefObj: {{ getObjectTypeName(item.objectTypeDefinition) }}
        </v-chip>
        <v-chip size="x-small" label v-if="item.objectTypeComparison">
          CompObj: {{ getObjectTypeName(item.objectTypeComparison) }}
        </v-chip>
      </v-list-item-subtitle>
    </template>

    <template #form-fields="{ model }">
      <v-text-field
        v-model="model.name"
        label="Rule Title"
        placeholder="e.g. Max 2 Bathrooms"
        :rules="[rules.required]"
        class="mb-2"
        density="compact"
        variant="outlined"
      ></v-text-field>
      <v-text-field
        v-model="model.description"
        label="Rule Desciprtion"
        placeholder="e.g. There can be a maximum of 2 bathrooms per blueprint"
        :rules="[rules.required]"
        class="mb-2"
        density="compact"
        variant="outlined"
      ></v-text-field>

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
import { rules } from "@/configuration/rules.js";
// api
const RULESETS_API_URL = API_CONFIG.ENDPOINTS.RULESETS;
const emptyRuleModel = ref({
  name: "",
  definition: "",
  comparison: null,
  objectTypeDefinition: null,
  objectTypeComparison: null,
});

const createLookup =
  (items, defaultText = "Not Assigned") =>
  (value) => {
    if (value === null || value === undefined) return defaultText;
    const item = items.find((i) => i.value === value);
    return item ? item.title : defaultText;
  };

const getDefinitionName = createLookup(definitionItems);
const getComparisonName = createLookup(comparisonItems);
const getObjectTypeName = createLookup(objectTypeItems, "N/A");
</script>
