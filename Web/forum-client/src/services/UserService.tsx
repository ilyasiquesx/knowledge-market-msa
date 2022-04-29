export interface User {
    id: string
    username: string,
    accessToken: string
}

export function isAuthenticated() {
    const user = getUser();
    return user?.accessToken != null;
}

export function getUser() {
    const userString = localStorage.getItem("current_user");
    let user = null;
    if (userString != null)
        user = JSON.parse(userString);

    return user;
}

export function setUser(user: User) {
    localStorage.setItem("current_user", JSON.stringify(user));
}

export function clearUser() {
    localStorage.removeItem("current_user");
}