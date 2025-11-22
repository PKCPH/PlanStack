const API_BASE_URL = "http://planstack.dk/api";

export const API_CONFIG = {
  BASE_URL: API_BASE_URL,

  ENDPOINTS: {
    LOGIN: `${API_BASE_URL}/auth/login`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    USERS: `${API_BASE_URL}/users`,
    BLUEPRINTS: `${API_BASE_URL}/blueprints`,
    BUILDING_STRUCTURES: `${API_BASE_URL}/buildingstructures`,
    COMPONENTS: `${API_BASE_URL}/components`,
    PROJECTS: `${API_BASE_URL}/projects`,
    STANDARDS: `${API_BASE_URL}/standards`,
    VALIDATE_BLUEPRINT: `${API_BASE_URL}/blueprints/validate`,
    RULESETS: `${API_BASE_URL}/rulesets`,
  },
};
