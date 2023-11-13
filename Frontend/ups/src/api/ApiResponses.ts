import { ExtendedProductDto, ExtendedSubProductDto, PagedList, ProductDto, SubProductDto, UserDto } from "./Dtos"

export type ApiResponse<T> = {
    statusCode: number,
    data?: T,
    errors: {[key: string]: string[]},
    success: boolean,
}

export type LoginResponse = {
    userDto: UserDto
}

export type ListUsersResponse = {
    users: PagedList<UserDto>
}

export type ListProductsResponse = {
    products: PagedList<ProductDto>
}

export type ListSubProductsResponse = {
    subProducts: PagedList<SubProductDto>
}

export type GetProductResponse = {
    product: ExtendedProductDto
}

export type GetSubProductResponse = {
    subProduct: ExtendedSubProductDto
}