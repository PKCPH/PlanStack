<template>
  <ResourceManager
    title="Standard"
    :api-endpoint="STANDARDS_API_URL"
    :empty-state-model="newStandard"
    @select="selectStandard"
  >
    <template #list-item="{ item }">
      <v-list-item-title>{{ item.name }}</v-list-item-title>
      <v-list-item-subtitle>
        Type: <strong>{{ item.type }}</strong>
      </v-list-item-subtitle>
      <v-list-item-subtitle class="mt-2"
        >{{ item.description || "No description" }}
      </v-list-item-subtitle>
    </template>

    <template #form-fields="{ model }">
      <v-text-field
        v-model="model.name"
        label="Standard Name"
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
        v-model="model.type"
        :items="standardTypeItems"
        label="Standard Type"
        :rules="[rules.required]"
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
const STANDARDS_API_URL = `${API_BASE_URL}/standards`;

const newStandard = ref({
  name: "",
  description: "",
  type: "",
});

const standardTypeItems = ref([
  { title: "Fire Safety", value: 0 },
  { title: "Accessibility", value: 1 },
  { title: "Energy Efficiency", value: 2 },
  { title: "Structural Integrity", value: 3 },
  { title: "Other", value: 99 },
]);

//form validation
const rules = {
  required: (v) => !!v || "This field is required",
};

const selectStandard = (standard) => {
  // for routering to ruleset
  console.log("Selected standard:", standard);
  // router.push(`/standards/${standard.id}`);
};
</script>
