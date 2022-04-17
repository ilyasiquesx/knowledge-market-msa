import axios from "axios";
import {getUser} from "./UserService";

const instance = axios.create();
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

export function getQuestions(request : any)
{
    return instance.get("http://localhost:4900/forum/questions/", {params: {pageSize: request.pageSize, page: request.page}})
}

export function getQuestionById(id : string)
{
    return instance.get(`http://localhost:4900/forum/questions/${id}`);
}