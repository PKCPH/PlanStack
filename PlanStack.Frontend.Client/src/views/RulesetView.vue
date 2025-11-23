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
          :disabled="isSaving"
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
        <v-list-item v-for="(item, index) in rulesets" :key="item.id || index">
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
              :disabled="isSaving"
            ></v-btn>
            <v-btn
              color="red-lighten-1"
              icon="mdi-delete"
              variant="text"
              size="small"
              @click.stop="removeRule(index, item)"
              title="Delete"
              :loading="isSaving && editingItemIndex === index"
              :disabled="isSaving"
            ></v-btn>
          </template>
        </v-list-item>
      </v-list>

      <v-divider v-if="!isLoading && rulesets.length > 0"></v-divider>
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
            :disabled="isSaving"
            >Cancel</v-btn
          >
          <v-btn
            color="success"
            variant="flat"
            @click="handleSaveRule"
            :loading="isSaving"
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
import { API_CONFIG } from "../components/api/config.js";
import definitionItems from "@/assets/enums/ruleSetDefinitionOptions.json";
import comparisonItems from "@/assets/enums/ruleSetComparisonOptions.json";
import objectTypeItems from "@/assets/enums/ruleSetObjectTypeOptions.json";
import { rules } from "@/configuration/rules.js";

const router = useRouter();
const route = useRoute();

// standardId from url
const standardId = ref(route.params.standardId);

// api
const STANDARDS_API_URL = API_CONFIG.ENDPOINTS.STANDARDS;
const BASE_RULESETS_API_URL = API_CONFIG.ENDPOINTS.RULESETS;

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

// give full rulesets from api
const hydrateRulesets = (apiRules) => {
  return (apiRules || []).map((apiRule) => {
    // full base rule from baseRulesets
    const baseRule = baseRulesets.value.find(
      (r) => r.id === apiRule.ruleSet.id
    );

    const cleanBaseRule = { ...baseRule };
    delete cleanBaseRule.displayName;

    return {
      ...apiRule,
      ruleSetId: apiRule.ruleSet.id,
      standardId: parseInt(standardId.value),
      // Overwrite ruleset from api
      ruleSet: cleanBaseRule || null,
    };
  });
};

// fetchstandards
const fetchStandardDetails = async () => {
  isLoading.value = true;
  error.value = null;
  try {
    // loading
    await fetchBaseRulesets();

    const url = `${STANDARDS_API_URL}/${standardId.value}`;
    const result = await apiFetch(url, {
      method: "GET",
      headers: { Host: "planstack.dk" },
    });
    if (!result.ok) throw new Error("API Error");
    standard.value = result.data;

    // hydrate rulesets
    rulesets.value = hydrateRulesets(result.data.ruleSets);
  } catch (e) {
    console.error(`Error fetching ${STANDARDS_API_URL}:`, e);
    error.value = "Failed to load standard details.";
  } finally {
    isLoading.value = false;
  }
};

// fetch base rulesets
const fetchBaseRulesets = async () => {
  // Prevent fetching if already loaded
  if (baseRulesets.value.length > 0) return;

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
    editingItem.value = JSON.parse(JSON.stringify(item));
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
const syncChangesToApi = async () => {
  isSaving.value = true;
  let success = false;

  // ruleset array
  const rulesetPayload = rulesets.value.map((localRule) => {
    return {
      userDefinedName: localRule.userDefinedName,
      definitionValue: localRule.definitionValue,
      comparisonValue: localRule.comparisonValue,
      standardId: localRule.standardId,
      ruleSetId: localRule.ruleSetId,
      ...(localRule.id && { id: localRule.id }),
    };
  });

  // full payload
  const payload = {
    id: standard.value.id,
    name: standard.value.name,
    description: standard.value.description,
    type: standard.value.type,
    isPublic: standard.value.isPublic,
    user: null, // As it was before
    ruleSets: rulesetPayload,
  };

  console.log("Syncing payload:", JSON.stringify(payload, null, 2));

  // send reqeust
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

    await fetchStandardDetails();
    showSnackbar("Changes saved successfully!", "success");
    success = true;
  } catch (e) {
    console.error("Error saving:", e);
    showSnackbar(`Error: ${e.message}`, "error");
    await fetchStandardDetails();
  } finally {
    isSaving.value = false;
    return success;
  }
};

const handleSaveRule = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  isSaving.value = true;

  // full item to save
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
    ...(editingItem.value.id && { id: editingItem.value.id }),
  };

  if (isEditing.value && editingItemIndex.value > -1) {
    rulesets.value.splice(editingItemIndex.value, 1, itemToSave);
  } else {
    rulesets.value.push(itemToSave);
  }

  //sync to api
  const success = await syncChangesToApi();

  if (success) {
    isEditDialogVisible.value = false;
  } else {
    isSaving.value = false;
  }
};

const removeRule = async (index, item) => {
  editingItemIndex.value = index;
  isSaving.value = true;

  // remove locallist
  const removedRule = rulesets.value.splice(index, 1)[0];
  try {
    await syncChangesToApi();
    showSnackbar("Rule removed successfully.");
  } catch (e) {
    rulesets.value.splice(index, 0, removedRule);
    showSnackbar("Failed to remove rule, reverting changes.", "error");
  } finally {
    editingItemIndex.value = -1;
  }
};

onMounted(() => {
  fetchStandardDetails();
});
</script>
