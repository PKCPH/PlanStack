<template>
  <v-container class="mt-10" max-width="500">
    <div class="text-center mb-6">
      <h1 class="text-h4 font-weight-bold text-grey-darken-3">Register</h1>
    </div>
    <v-form ref="formRef">
      <v-text-field
        v-model="registerForm.userName"
        label="Username"
        :rules="registerRules.required"
        density="compact"
        class="mb-2"
      />

      <v-text-field
        v-model="registerForm.email"
        label="Email"
        :rules="[...registerRules.required, ...registerRules.email]"
        density="compact"
        class="mb-2"
      />

      <v-text-field
        v-model="registerForm.password"
        label="Password"
        type="password"
        :rules="registerRules.required"
        density="compact"
        class="mb-2"
      />

      <v-text-field
        v-model="registerForm.confirmPassword"
        label="Confirm Password"
        type="password"
        :rules="registerRules.confirmPassword"
        density="compact"
        class="mb-2"
      />

      <v-alert v-if="registerError" type="error" class="mb-4">
        {{ registerError }}
      </v-alert>

      <v-btn color="primary" :loading="isRegistering" @click="register" block>
        Register
      </v-btn>
    </v-form>
  </v-container>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { setToken } from "../components/api/auth.js";

const CORS_PROXY_URL = "";
const API_BASE_URL = "http://planstack.dk/api";
const REGISTER_API_URL = `${API_BASE_URL}/auth/register`;
const LOGIN_API_URL = `${API_BASE_URL}/auth/login`;

const formRef = ref(null);
const isRegistering = ref(false);
const registerError = ref(null);

const router = useRouter();

const registerForm = ref({
  userName: "",
  email: "",
  password: "",
  confirmPassword: "",
});

const registerRules = {
  required: [(v) => !!v || "This field is required"],
  email: [(v) => /.+@.+\..+/.test(v) || "Email must be valid"],
  confirmPassword: [
    (v) => v === registerForm.value.password || "Passwords do not match",
  ],
};

const register = async () => {
  registerError.value = null;

  // validate form
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  isRegistering.value = true;

  try {
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(
      REGISTER_API_URL
    )}`;

    const response = await fetch(REGISTER_API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Host: "planstack.dk",
      },
      body: JSON.stringify({
        userName: registerForm.value.userName,
        email: registerForm.value.email,
        password: registerForm.value.password,
        confirmPassword: registerForm.value.confirmPassword,
      }),
    });

    if (response.status === 201) {
      console.log("User registered successfully");

      await login(registerForm.value.email, registerForm.value.password);

      return;
    }

    let errorMessage = `Error: ${response.status} ${response.statusText}`;

    try {
      const errorData = await response.json();
      if (errorData.errors) {
        // ASP.NET model validation errors
        errorMessage = Object.values(errorData.errors).join("\n");
      } else if (Array.isArray(errorData.errors)) {
        // Identity creation errors (password too weak, etc.)
        errorMessage = errorData.errors.join("\n");
      } else if (errorData.title) {
        errorMessage = errorData.title;
      }
    } catch (e) {}

    throw new Error(errorMessage);
  } catch (err) {
    console.error("Register error:", err);
    registerError.value = err.message;
  } finally {
    isRegistering.value = false;
  }
};

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
    const proxiedUrl = `${CORS_PROXY_URL}${encodeURIComponent(LOGIN_API_URL)}`;

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
