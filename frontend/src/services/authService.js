const TOKEN_KEY = "token";
const ROLE_KEY = "role";
const USER_ID_KEY = "userId";
const USER_NAME_KEY = "userName";

export function saveSession(authData) {
  localStorage.setItem(TOKEN_KEY, authData.token);
  localStorage.setItem(ROLE_KEY, authData.role);

  if (authData.userId !== undefined && authData.userId !== null) {
    localStorage.setItem(USER_ID_KEY, String(authData.userId));
  }

  if (authData.userName) {
    localStorage.setItem(USER_NAME_KEY, authData.userName);
  }
}

export function clearSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(ROLE_KEY);
  localStorage.removeItem(USER_ID_KEY);
  localStorage.removeItem(USER_NAME_KEY);
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function getRole() {
  return localStorage.getItem(ROLE_KEY);
}

export function getHomeRouteByRole(role) {
  if (role === "Admin") {
    return "/admin";
  }

  if (role === "Officer") {
    return "/employee";
  }

  return "/";
}

export function isTokenExpired(token) {
  const payload = parseJwtPayload(token);
  if (!payload || !payload.exp) {
    return true;
  }

  const expiresAtMs = Number(payload.exp) * 1000;
  return Number.isNaN(expiresAtMs) || Date.now() >= expiresAtMs;
}

function parseJwtPayload(token) {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) {
      return null;
    }

    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((char) => `%${(`00${char.charCodeAt(0).toString(16)}`).slice(-2)}`)
        .join("")
    );

    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}
