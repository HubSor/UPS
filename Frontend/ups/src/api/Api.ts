import axios from "axios";
import { ApiResponse, FindCompanyClientResponse, FindPersonClientResponse, GetProductResponse, GetSubProductResponse, ListProductsResponse, ListSubProductsResponse, ListUsersResponse, LoginResponse, UpsertClientResponse } from "./ApiResponses";
import { AddOptionRequest, AddParameterRequest, AddProductRequest, AddSubProductRequest, AddUserRequest, AssignSubProductRequest, DeleteOptionRequest, DeleteParameterRequest, DeleteProductRequest, DeleteSubProductRequest, DeleteUserRequest, EditParameterRequest, EditProductRequest, EditSubProductAssignmentRequest, EditSubProductRequest, EditUserRequest, FindClientRequest, GetProductRequest, GetSubProductRequest, ListProductsRequest, ListSubProductsRequest, ListUsersRequest, LoginRequest, SaveSaleRequest, UnassignSubProductsRequest, UpsertClientRequest } from "./ApiRequests";
import { AuthHelpers } from "../helpers/AuthHelper";
import { toastAuthError } from "../helpers/ToastHelpers";

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

    if (response.statusCode === 401)
        toastAuthError();

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

    static async ListUsers(request: ListUsersRequest) {
        return await getApiResponse<ListUsersRequest, ListUsersResponse>(request, this.url + "/users/list")
    }

    static async EditUser(request: EditUserRequest) {
        return await getApiResponse<EditUserRequest, undefined>(request, this.url + "/users/edit")
    }

    static async DeleteUser(request: DeleteUserRequest) {
        return await getApiResponse<DeleteUserRequest, undefined>(request, this.url + "/users/delete")
    }

    static async AddProduct(request: AddProductRequest){
        return await getApiResponse<AddProductRequest, undefined>(request, this.url + '/products/add')
    }

    static async EditProduct(request: EditProductRequest) {
        return await getApiResponse<EditProductRequest, undefined>(request, this.url + '/products/edit')
    }

    static async DeleteProduct(request: DeleteProductRequest) {
        return await getApiResponse<DeleteProductRequest, undefined>(request, this.url + '/products/delete')
    }

    static async ListProducts(request: ListProductsRequest) {
        return await getApiResponse<ListProductsRequest, ListProductsResponse>(request, this.url + '/products/list')
    }

    static async AddSubProduct(request: AddSubProductRequest) {
        return await getApiResponse<AddSubProductRequest, undefined>(request, this.url + '/products/subproducts/add')
    }

    static async EditSubProduct(request: EditSubProductRequest) {
        return await getApiResponse<EditSubProductRequest, undefined>(request, this.url + '/products/subproducts/edit')
    }

    static async DeleteSubProduct(request: DeleteSubProductRequest) {
        return await getApiResponse<DeleteSubProductRequest, undefined>(request, this.url + '/products/subproducts/delete')
    }

    static async ListSubProducts(request: ListSubProductsRequest) {
        return await getApiResponse<ListSubProductsRequest, ListSubProductsResponse>(request, this.url + '/products/subproducts/list')
    }

    static async AssignSubProduct(request: AssignSubProductRequest) {
        return await getApiResponse<AssignSubProductRequest, undefined>(request, this.url + '/products/subproducts/assign')
    }

    static async UnassignSubProducts(request: UnassignSubProductsRequest) {
        return await getApiResponse<UnassignSubProductsRequest, undefined>(request, this.url + "/products/subproducts/unassign")
    }

    static async EditSubProductAssignment(request: EditSubProductAssignmentRequest) {
        return await getApiResponse<EditSubProductAssignmentRequest, undefined>(request, this.url + "/products/subproducts/assignments/edit")
    }

    static async GetProduct(request: GetProductRequest) {
        return await getApiResponse<GetProductRequest, GetProductResponse>(request, this.url + "/products/get")
    }

    static async GetSubProduct(request: GetSubProductRequest) {
        return await getApiResponse<GetSubProductRequest, GetSubProductResponse>(request, this.url + "/products/subproducts/get")
    }

    static async AddParameter(request: AddParameterRequest) {
        return await getApiResponse<AddParameterRequest, undefined>(request, this.url + "/parameters/add")
    }

    static async EditParameter(request: EditParameterRequest) {
        return await getApiResponse<EditParameterRequest, undefined>(request, this.url + "/parameters/edit")
    }

    static async DeleteParameter(request: DeleteParameterRequest) {
        return await getApiResponse<DeleteParameterRequest, undefined>(request, this.url + "/parameters/delete")
    }

    static async AddOption(request: AddOptionRequest) {
        return await getApiResponse<AddOptionRequest, undefined>(request, this.url + "/parameters/options/add")
    }

    static async DeleteOption(request: DeleteOptionRequest) {
        return await getApiResponse<DeleteOptionRequest, undefined>(request, this.url + "/parameters/options/delete")
    }

    static async UpsertClient(request: UpsertClientRequest) {
        return await getApiResponse<UpsertClientRequest, UpsertClientResponse>(request, this.url + "/clients/upsert")
    }

    static async FindPersonClient(request: FindClientRequest) {
        return await getApiResponse<FindClientRequest, FindPersonClientResponse>(request, this.url + "/clients/people/find")
    }

    static async FindCompanyClient(request: FindClientRequest) {
        return await getApiResponse<FindClientRequest, FindCompanyClientResponse>(request, this.url + "/clients/companies/find")
    }

    static async SaveSale(request: SaveSaleRequest) {
        return await getApiResponse<SaveSaleRequest, undefined>(request, this.url + "/sales/save")
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