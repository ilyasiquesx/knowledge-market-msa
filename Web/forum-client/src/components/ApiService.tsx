import axios from "axios";
import {clearUser, getUser} from "./UserService";
import {toast} from "react-toastify";

const instance = axios.create();
const baseUrl = process.env.REACT_APP_GATEWAY_URL;

instance.interceptors.response.use((r => {
    return r;
}), er => {
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
    return instance.post(`${baseUrl}/auth/Register`, body)
}

export function loginApi(body: any) {
    return instance.post(`${baseUrl}/auth/Login`, body)
}

export function getQuestions(request: any) {
    return instance.get(`${baseUrl}/forum/questions/`, {
        params: {
            pageSize: request.pageSize,
            page: request.page
        }
    })
}

export function getQuestionById(id: string) {
    return instance.get(`${baseUrl}/forum/questions/${id}`);
}

export function postAnswer(body: any) {
    return instance.post(`${baseUrl}/forum/Answers`, body)
}

export function postQuestion(body: any) {
    return instance.post(`${baseUrl}/forum/Questions`, body)
}

export function putQuestion(id: string, body: any) {
    return instance.put(`${baseUrl}/forum/Questions/${id}`, body)
}

export function deleteQuestion(id: string) {
    return instance.delete(`${baseUrl}/forum/Questions/${id}`)
}

export function getNotifications() {
    return instance.get(`${baseUrl}/notifications`)
}

export function putNotifications() {
    return instance.put(`${baseUrl}/notifications`)
}

export function putMailing(userId: string) {
    return instance.put(`${baseUrl}/mailing/${userId}`)
}