export const rules = {
  required: (v) => !!v || "This field is required",
  requiredSelect: (v) => {
    return (v !== null && v !== undefined) || "This field is required";
  },
  requiredFile: (v) => !!v || "A file is required",
  email: (v) => /.+@.+\..+/.test(v) || "Email must be valid",
  confirmPassword: (v) =>
    v === registerForm.value.password || "Passwords do not match",
};
