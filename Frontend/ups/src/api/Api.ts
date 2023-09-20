import axios from "axios";
import { ApiResponse, LoginResponse } from "./ApiResponses";
import { LoginRequest } from "./ApiRequests";

axios.interceptors.response.use(response => {
    return response;
}, error => {
    if ([401, 403].includes(error.response.status)) {
        // wyloguj
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

    await axios.post(url, request, {
        validateStatus: status => status <= 500
    }).catch(error => {
        if (error.response)
            response = {
                statusCode: error.response.status ?? 400,
                data: error.response.data.message.data,
                errors: error.response.data.message.errors ?? {},
                success: error.response.data.message.success ?? false
            }
        else
            console.error('Api Error: ', error.message)
    });

    return response;
}

export class Api {
    private static url =  process.env.REACT_APP_BACKEND_URL;

    static async Login(request: LoginRequest) {
        return await getApiResponse<LoginRequest, LoginResponse>(request, this.url + "/user/login"); 
    }

    static async Logout() {
        return await getApiResponse<null, null>(null, this.url + "/user/logout");
    }
}