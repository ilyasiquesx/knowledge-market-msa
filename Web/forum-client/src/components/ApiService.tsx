import axios from "axios";
import {clearUser, getUser} from "./UserService";
import {toast} from "react-toastify";

const instance = axios.create();

instance.interceptors.response.use((r => r), er => {
    toast.error(er.response.data.message, {
        position: "top-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
    });

    if (er.response.status == 403 || er.response.status == 401) {
        clearUser();
        window.location.assign("/auth");
    }

    return er;
})

instance.interceptors.request.use((r) => {
    r.headers = {
        Authorization: `Bearer ${getUser()?.accessToken}`
    }
    return r;
});

export function registerApi(body: any) {
    return instance.post("http://localhost:4900/auth/Register", body)
}

export function loginApi(body: any) {
    return instance.post("http://localhost:4900/auth/Login", body)
}

export function getQuestions(request: any) {
    return instance.get("http://localhost:4900/forum/questions/", {
        params: {
            pageSize: request.pageSize,
            page: request.page
        }
    })
}

export function getQuestionById(id: string) {
    return instance.get(`http://localhost:4900/forum/questions/${id}`);
}

export function postAnswer(body: any) {
    return instance.post("http://localhost:4900/forum/Answers", body)
}

export function postQuestion(body: any) {
    return instance.post("http://localhost:4900/forum/Questions", body)
}

export function getNotifications() {
    return instance.get("http://localhost:4900/notifications")
}