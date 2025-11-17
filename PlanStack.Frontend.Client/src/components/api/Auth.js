import { ref, computed } from "vue";

const token = ref(localStorage.getItem("token") || null);

export const isLoggedIn = computed(() => !!token.value);

export function setToken(newToken) {
  token.value = newToken;
  if (newToken) {
    localStorage.setItem("token", newToken);
  } else {
    localStorage.removeItem("token");
  }
}

export function parseJwt(token) {
  if (!token) return null;
  try {
    const base64Url = token.split('.')[1];
    // convert base64url to base64
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    // decode
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error('Invalid JWT', e);
    return null;
  }
}

export function getCurrentUser() {
  if (!token.value) return null;
  const payload = parseJwt(token.value);
  if (!payload) return null;

  return {
    userId: payload.sub,             
    username: payload.unique_name,   
    email: payload.email,            
    roles: payload.role
      ? Array.isArray(payload.role)
        ? payload.role
        : [payload.role]
      : [],
  };
}

export async function apiFetch(url, options = {}) {
  const headers = options.headers || {};

  // add Authorization if logged in
  if (isLoggedIn.value && token.value) {
    headers['Authorization'] = `Bearer ${token.value}`;
  }

  options.headers = headers;

  try {
    const response = await fetch(url, options);

    if (response.status === 401) {
      console.warn("Unauthorized");
      setToken(null); 
      if (typeof window !== 'undefined') {
        window.location.href = '/session-timeout';
      }
    }

    // parse JSON if applicable
    let data = null;
    let isJson = false;
    const contentType = response.headers.get("content-type");
    if (contentType && contentType.includes("application/json")) {
      isJson = true;
      try {
        data = await response.json();     
      } catch {}
    }

    // Return a response object compatible with fetchStructures
    return {
      ok: response.ok,
      status: response.status,
      statusText: response.statusText,
      data,
      json: isJson ? async () => data : async () => { throw new Error("Response is not JSON"); }
    };
  } catch (error) {
    console.error("API fetch error:", error);
    return { ok: false, status: 0, data: null, error };
  }
}