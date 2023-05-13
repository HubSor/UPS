import axios, { AxiosResponse } from "axios";
import { AddProductRequest } from "./ApiRequests";
import { ProductsResponse } from "./ApiResponses";

export class Api {
    private static url =  process.env.REACT_APP_BACKEND_URL;

    static async AddProduct(req: AddProductRequest) : Promise<AxiosResponse<null>> {
        return await axios.post<null>(this.url + "/test/addproduct", req);
    }

    static async GetProducts() : Promise<AxiosResponse<ProductsResponse>> {
        return await axios.get<ProductsResponse>(this.url + "/test/viewproducts");
    }

    static async Test() : Promise<AxiosResponse<null>> {
        return await axios.get<null>(this.url + "/test/get");
    }
}