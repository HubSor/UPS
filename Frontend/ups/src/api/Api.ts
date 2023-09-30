import axios from "axios";
import { ApiResponse, LoginResponse } from "./ApiResponses";
import { AddUserRequest, LoginRequest } from "./ApiRequests";
import { AuthHelpers } from "../helpers/AuthHelper";

axios.defaults.withCredentials = true;

axios.interceptors.response.use(response => {
    return response;
}, error => {
    if ([401, 403].includes(error.response.status)) {
        Api.Logout();
        AuthHelpers.ClearAllData();
    }
    return error;
});

async function getApiResponse<R, T>(request: R, url: string): Promise<ApiResponse<T>>{
    let response: ApiResponse<T> = {
        statusCode: 400,
        data: undefined,
        errors: {},
        success: false
    };

    await axios.post(url, request ?? {}, {
        validateStatus: status => status <= 500,    
        headers: {
            "Content-Type": "application/json"
        }    
    }).then(res => {
        response = {
            statusCode: res.status ?? 400,
            data: res.data.data,
            errors: res.data.errors ?? {},
            success: res.data.success ?? false
        }
    }).catch(error => {
        if (error.response)
            response = {
                statusCode: error.response.status ?? 400,
                data: error.response.data.data,
                errors: error.response.data.errors ?? {},
                success: error.response.data.success ?? false
            }
        else
            console.error('Api Error: ', error.message)
    });

    return response;
}

export class Api {
    private static url =  process.env.REACT_APP_BACKEND_URL;

    static async Login(request: LoginRequest) {
        return await getApiResponse<LoginRequest, LoginResponse>(request, this.url + "/users/login"); 
    }

    static async Logout() {
        return await getApiResponse<undefined, undefined>(undefined, this.url + "/users/logout");
    }

    static async AddUser(request: AddUserRequest) {
        return await getApiResponse<AddUserRequest, undefined>(request, this.url + "/users/add");
    }

    static async Session() {
        const abc = await axios.post(this.url + "/users/session", {}, {
            validateStatus: status => status <= 500,    
            headers: {
                "Content-Type": "application/json"
            }    
        }).then(r => {
            return r.data as boolean;
        }).catch(e => {
            return false;
        })

        return abc;
    }
}