import { ref } from "vue";
import axios from "axios";

const BLUEPRINTS_API_URL = "http://planstack.dk/api/blueprints";

export function useBlueprintsApi() {
  const isSaving = ref(false);
  const saveMessage = ref("");
  const lastSavedBlueprint = ref(null);

  //Sends request creates new blueprint.

  const saveBlueprint = async (blueprintData) => {
    if (!blueprintData.name) {
      saveMessage.value = "Error: Blueprint Name is required.";
      return;
    }

    isSaving.value = true;
    saveMessage.value = "Saving blueprint...";
    lastSavedBlueprint.value = null;

    try {
      const response = await axios.post(BLUEPRINTS_API_URL, blueprintData);
      console.log("API Response:", response.data);

      lastSavedBlueprint.value = response.data;
      saveMessage.value = `Blueprint saved successfully! ID: ${response.data.id || "N/A"}`;
      return response.data;
    } catch (error) {
      console.error("Error saving blueprint:", error);
      saveMessage.value = `Error saving blueprint: ${error.message || "An unknown error occurred"}.`;
      // Rethrow if error
      throw error;
    } finally {
      isSaving.value = false;
    }
  };

  return {
    isSaving,
    saveMessage,
    lastSavedBlueprint,
    saveBlueprint,
  };
}
