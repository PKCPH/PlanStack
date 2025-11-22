<template>
  <v-container class="mt-10" max-width="500">
    <v-card class="pa-6" elevation="3">
      <h1 class="text-h5 font-weight-bold mb-4">Account</h1>

      <div v-if="user">
        <v-avatar color="primary" size="64" class="mb-4">
          <span class="text-h5">{{
            user.username?.charAt(0).toUpperCase()
          }}</span>
        </v-avatar>
        <h2 class="text-h6 mb-2">Hello, {{ user.username }}</h2>
        <v-divider class="my-3"></v-divider>
        <v-list dense>
          <v-list-item>
            <v-list-item-title>Email:</v-list-item-title>
            <v-list-item-subtitle>{{ user.email }}</v-list-item-subtitle>
          </v-list-item>
          <v-list-item>
            <v-list-item-title>Roles:</v-list-item-title>
            <v-list-item-subtitle>{{
              user.roles.join(", ")
            }}</v-list-item-subtitle>
          </v-list-item>
        </v-list>
        <v-btn color="primary" class="mt-5" @click="showDialog = true" block>
          Change Password
        </v-btn>
      </div>

      <div v-else>
        <v-skeleton-loader type="list-item-avatar"></v-skeleton-loader>
        <p class="mt-4">Loading user info...</p>
      </div>
    </v-card>

    <!-- Change Password Dialog -->
    <v-dialog v-model="showDialog" max-width="400">
      <v-card>
        <v-card-title>Change Password</v-card-title>
        <v-card-text>
          <v-form ref="passwordFormRef">
            <v-text-field
              v-model="passwordForm.currentPassword"
              label="Current Password"
              type="password"
              required
            />
            <v-text-field
              v-model="passwordForm.newPassword"
              label="New Password"
              type="password"
              required
            />
            <v-text-field
              v-model="passwordForm.confirmNewPassword"
              label="Confirm New Password"
              type="password"
              required
              :error-messages="passwordError"
            />
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text @click="showDialog = false">Cancel</v-btn>
          <v-btn
            color="primary"
            @click="submitPasswordChange"
            :loading="isChangingPassword"
            >Change</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { getCurrentUser, apiFetch } from "../components/api/auth.js";

const user = ref(null);
const showDialog = ref(false);
const passwordForm = ref({
  currentPassword: "",
  newPassword: "",
  confirmNewPassword: "",
});
const passwordError = ref("");
const isChangingPassword = ref(false);
const passwordFormRef = ref(null);

onMounted(() => {
  user.value = getCurrentUser();
});

function changePassword() {
  showDialog.value = true;
}

async function submitPasswordChange() {
  passwordError.value = "";
  if (passwordForm.value.newPassword.length < 7) {
    passwordError.value = "Password must be at least 7 characters.";
    return;
  }
  if (
    passwordForm.value.newPassword !== passwordForm.value.confirmNewPassword
  ) {
    passwordError.value = "New passwords do not match.";
    return;
  }

  isChangingPassword.value = true;

  const CORS_PROXY_URL = "";
  const API_BASE_URL = "http://planstack.dk/api";
  const USER_API_URL = `${API_BASE_URL}/users`;
  const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(USER_API_URL)}`;

  try {
    const response = await apiFetch(proxiedUrl, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        currentPassword: passwordForm.value.currentPassword,
        newPassword: passwordForm.value.newPassword,
        confirmNewPassword: passwordForm.value.confirmNewPassword,
      }),
    });

    if (response.ok) {
      showDialog.value = false;
      passwordForm.value.currentPassword = "";
      passwordForm.value.newPassword = "";
      passwordForm.value.confirmNewPassword = "";
      alert("Password changed successfully!");
    } else {
      const data = await response.json();
      passwordError.value = data.errorMessage || "Failed to change password.";
    }
  } catch (err) {
    passwordError.value = "Network error.";
  } finally {
    isChangingPassword.value = false;
  }
}
</script>
