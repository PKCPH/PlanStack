<template>
  <v-container class="py-10">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">
        Rulesets for: {{ standard.name || "..." }}
      </h1>
      <h2 class="text-subtitle-1 text-grey-darken-1">
        {{ standard.description }}
      </h2>
    </div>

    <v-sheet max-width="800" class="mx-auto" rounded="lg" border>
      <div class="d-flex justify-space-between align-center pa-4">
        <v-list-subheader class="pa-0">Rules</v-list-subheader>
        <v-btn
          color="secondary"
          @click="openEditDialog(null)"
          prepend-icon="mdi-plus"
          size="small"
          variant="tonal"
        >
          Add Rule
        </v-btn>
      </div>

      <v-divider></v-divider>

      <!-- loading -->
      <v-skeleton-loader
        v-if="isLoading"
        type="list-item-two-line@3"
      ></v-skeleton-loader>

      <!-- error -->
      <v-alert
        v-if="error"
        type="error"
        title="Error loading data"
        :text="error"
        class="ma-4"
        variant="tonal"
      ></v-alert>

      <!-- if no rules -->
      <v-alert
        v-if="!isLoading && !error && rulesets.length === 0"
        type="info"
        text="No rules found for this standard. You can add one above."
        class="ma-4"
        variant="tonal"
      ></v-alert>

      <!-- list of rules -->
      <v-list
        v-if="!isLoading && rulesets.length > 0"
        lines="two"
        style="max-height: 500px; overflow-y: auto"
      >
        <v-list-item v-for="(item, index) in rulesets" :key="index">
          <v-list-item-title>{{ item.userDefinedName }}</v-list-item-title>
          <v-list-item-subtitle v-if="item.ruleSet">
            <strong>Rule:</strong>
            {{ getDefinitionName(item.ruleSet.definition) }}
            {{ getComparisonName(item.ruleSet.comparison) }}
          </v-list-item-subtitle>
          <v-list-item-subtitle class="mt-2">
            <strong>Definition Value:</strong> {{ item.definitionValue }} |
            <strong>Comparison Value:</strong> {{ item.comparisonValue }}
          </v-list-item-subtitle>

          <!-- actions -->
          <template v-slot:append>
            <v-btn
              color="grey-darken-1"
              icon="mdi-pencil"
              variant="text"
              size="small"
              class="mr-1"
              @click.stop="openEditDialog(item, index)"
              title="Edit"
            ></v-btn>
            <v-btn
              color="red-lighten-1"
              icon="mdi-delete"
              variant="text"
              size="small"
              @click.stop="removeRule(index)"
              title="Delete"
            ></v-btn>
          </template>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>

      <!-- save all -->
      <div class="pa-4 d-flex">
        <v-spacer></v-spacer>
        <v-btn
          color="success"
          @click="handleSaveAll"
          :loading="isSaving"
          prepend-icon="mdi-content-save"
          size="large"
          variant="flat"
        >
          Save All Changes
        </v-btn>
      </div>
    </v-sheet>

    <!-- create edit dialog -->
    <v-dialog v-model="isEditDialogVisible" max-width="800" persistent>
      <v-card>
        <v-card-title class="pa-4">
          <span class="text-h5">{{
            isEditing ? "Edit Rule" : "Add New Rule"
          }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="formRef">
            <v-text-field
              v-model="editingItem.userDefinedName"
              label="Rule Name"
              :rules="[rules.required]"
              class="mb-2"
              density="compact"
            ></v-text-field>

            <v-autocomplete
              v-model="editingItem.ruleSetId"
              :items="baseRulesets"
              :loading="isLoadingBaseRulesets"
              item-title="displayName"
              item-value="id"
              label="Base Rule"
              :rules="[rules.requiredSelect]"
              class="mb-2"
              density="compact"
            >
              <template v-slot:item="{ props, item }">
                <v-list-item v-bind="props" :title="item.raw.displayName">
                  <v-list-item-subtitle>
                    Defines:
                    {{ getObjectTypeName(item.raw.objectTypeDefinition) }}
                  </v-list-item-subtitle>
                  <v-list-item-subtitle>
                    Compares:
                    {{ getObjectTypeName(item.raw.objectTypeComparison) }}
                  </v-list-item-subtitle>
                </v-list-item>
              </template>
            </v-autocomplete>

            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="editingItem.definitionValue"
                  label="Definition Value"
                  type="number"
                  :rules="[rules.required]"
                  class="mb-2"
                  density="compact"
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="editingItem.comparisonValue"
                  label="Comparison Value"
                  type="number"
                  :rules="[rules.required]"
                  class="mb-2"
                  density="compact"
                ></v-text-field>
              </v-col>
            </v-row>
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
          <v-btn color="success" variant="flat" @click="handleSaveRule"
            >Save Rule</v-btn
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
import { ref, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { apiFetch } from "@/components/api/Auth.js";

const router = useRouter();
const route = useRoute();

// standardId from url
const standardId = ref(route.params.standardId);

// api
const API_BASE_URL = "http://planstack.dk/api";
const STANDARDS_API_URL = `${API_BASE_URL}/standards`;
const BASE_RULESETS_API_URL = `${API_BASE_URL}/rulesets`;

// states
const standard = ref({});
const rulesets = ref([]);
const baseRulesets = ref([]);
const isLoading = ref(false);
const isLoadingBaseRulesets = ref(false);
const isSaving = ref(false);
const error = ref(null);
const formRef = ref(null);

const isEditDialogVisible = ref(false);
const isEditing = ref(false);
const editingItem = ref({});
const editingItemIndex = ref(-1);

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
  return item ? item.title : "N/A";
};
const getComparisonName = (value) => {
  const item = comparisonItems.value.find((item) => item.value === value);
  return item ? item.title : "N/A";
};
const getObjectTypeName = (value) => {
  if (value === null) return "N/A";
  const item = objectTypeItems.value.find((item) => item.value === value);
  return item ? item.title : "N/A";
};

// snackbar
const snackbar = ref(false);
const snackbarText = ref("");
const snackbarColor = ref("success");
const showSnackbar = (text, color = "success") => {
  snackbarText.value = text;
  snackbarColor.value = color;
  snackbar.value = true;
};

// fetchstandards
const fetchStandardDetails = async () => {
  isLoading.value = true;
  error.value = null;
  try {
    const url = `${STANDARDS_API_URL}/${standardId.value}`;
    const result = await apiFetch(url, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });
    if (!result.ok) throw new Error("API Error");
    standard.value = result.data;
    // store local rulesets
    rulesets.value = result.data.ruleSets || [];
  } catch (e) {
    console.error(`Error fetching ${STANDARDS_API_URL}:`, e);
    error.value = "Failed to load standard details.";
  } finally {
    isLoading.value = false;
  }
};

// fetch base rulesets
const fetchBaseRulesets = async () => {
  isLoadingBaseRulesets.value = true;
  try {
    const result = await apiFetch(BASE_RULESETS_API_URL, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });
    if (!result.ok) throw new Error("API Error");

    baseRulesets.value =
      (result.data.entities || []).map((rule) => ({
        ...rule,
        displayName: `${getDefinitionName(
          rule.definition
        )} ${getComparisonName(rule.comparison)}`,
      })) || [];
  } catch (error) {
    console.error(`Error fetching ${BASE_RULESETS_API_URL}:`, error);
    showSnackbar("Failed to load base rules.", "error");
  } finally {
    isLoadingBaseRulesets.value = false;
  }
};

const openEditDialog = (item, index) => {
  if (item) {
    //edit
    isEditing.value = true;
    editingItem.value = Object.assign({}, item);
    editingItemIndex.value = index;
  } else {
    //create
    isEditing.value = false;
    editingItem.value = {
      userDefinedName: "",
      definitionValue: null,
      comparisonValue: null,
      ruleSetId: null,
      standardId: parseInt(standardId.value),
    };
    editingItemIndex.value = -1;
  }
  isEditDialogVisible.value = true;
};

const handleSaveRule = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  const baseRule = baseRulesets.value.find(
    (r) => r.id === editingItem.value.ruleSetId
  );

  const cleanBaseRule = { ...baseRule };
  delete cleanBaseRule.displayName;

  const itemToSave = {
    userDefinedName: editingItem.value.userDefinedName,
    definitionValue: editingItem.value.definitionValue,
    comparisonValue: editingItem.value.comparisonValue,
    ruleSetId: editingItem.value.ruleSetId,
    standardId: editingItem.value.standardId,

    ruleSet: cleanBaseRule,

    // id if editing existing
    ...(editingItem.value.id && { id: editingItem.value.id }),
  };

  if (isEditing.value && editingItemIndex.value > -1) {
    //update existing
    rulesets.value.splice(editingItemIndex.value, 1, itemToSave);
  } else {
    //add new
    rulesets.value.push(itemToSave);
  }

  isEditDialogVisible.value = false;
};

const removeRule = (index) => {
  rulesets.value.splice(index, 1);
  showSnackbar("Rule removed locally. Press 'Save All Changes' to commit.");
};

// payload for api
const handleSaveAll = async () => {
  isSaving.value = true;

  // ruleset array
  const rulesetPayload = rulesets.value.map((localRule) => {
    return {
      userDefinedName: localRule.userDefinedName,
      definitionValue: localRule.definitionValue,
      comparisonValue: localRule.comparisonValue,
      standardId: localRule.standardId,
      ruleSetId: localRule.ruleSetId,
      // add id if exists
      ...(localRule.id && { id: localRule.id }),
    };
  });

  // standard payload
  const payload = {
    ...standard.value,

    //overwrite rulesets
    ruleSets: rulesetPayload,
  };

  // remove fields if update
  delete payload.createdAt;
  delete payload.updatedAt;
  payload.user = null; // null for now

  console.log("Saving payload:", JSON.stringify(payload, null, 2));

  try {
    const url = `${STANDARDS_API_URL}/${standardId.value}`;
    const response = await apiFetch(url, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const errorMsg = response.data?.message || "error occurred";
      throw new Error(errorMsg);
    }

    showSnackbar("All rules saved");
    await fetchStandardDetails();
  } catch (e) {
    console.error("Error saving:", e);
    showSnackbar(`Error: ${e.message}`, "error");
  } finally {
    isSaving.value = false;
  }
};

onMounted(() => {
  fetchStandardDetails();
  fetchBaseRulesets();
});
</script>
