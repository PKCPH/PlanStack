<template>
  <v-container class="mt-10" max-width="500">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">Login</h1>
    </div>

    <v-form ref="formRef">
      <v-text-field
        v-model="loginForm.email"
        label="Email"
        type="email"
        required
        prepend-inner-icon="mdi-email"
      />

      <v-text-field
        v-model="loginForm.password"
        label="Password"
        type="password"
        required
        prepend-inner-icon="mdi-lock"
      />

      <v-btn
        class="mt-4"
        color="primary"
        :loading="isLoggingIn"
        @click="login(loginForm.email, loginForm.password)"
        block
      >
        Login
      </v-btn>

      <div v-if="loginError" class="text-red mt-4">
        {{ loginError }}
      </div>
    </v-form>

    <div class="text-center mt-6">
      <span>New to PlanStack?</span>
      <router-link to="/register" class="ml-1">Create an account</router-link>
    </div>
  </v-container>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { setToken } from "../components/api/auth.js";
import { API_CONFIG } from "../components/api/config.js";

const LOGIN_API_URL = API_CONFIG.ENDPOINTS.LOGIN;

const formRef = ref(null);
const isRegistering = ref(false);
const registerError = ref(null);

const router = useRouter();

const loginForm = ref({
  email: "",
  password: "",
});

const isLoggingIn = ref(false);
const loginError = ref(null);

const login = async (email, password) => {
  loginError.value = null;
  isLoggingIn.value = true;

  const payload = {
    email: email,
    password: password,
  };

  try {
    const response = await fetch(LOGIN_API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify(payload),
    });

    const data = await response.json();

    if (response.ok && data.isAuthSuccessful) {
      console.log("Login successful");

      // save token
      setToken(data.token);

      // redirect to home
      router.push("/");

      return true;
    }

    // handle errors
    loginError.value = data.errorMessage || "Login failed";
    return false;
  } catch (error) {
    console.error("Login error:", error);
    loginError.value = error.message;
    return false;
  } finally {
    isLoggingIn.value = false;
  }
};
</script>
