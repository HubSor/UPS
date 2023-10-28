export type UserDto = {
    username: string
    roles: string[]
    id: number
}

export enum RoleEnum {
    Administrator = 0,
    Seller = 1,
    UserManager = 2,
    ProductManager = 3,
} 

export type PaginationDto = {
    pageSize: number,
    pageIndex: number
}

export type ResultPaginationDto = PaginationDto & {
    totalPages: number,
    totalCount: number,
    count: number
}

export type PagedList<T> = {
    items: T[]
    pagination: ResultPaginationDto
}

export enum ProductStatusEnum {
    NotOffered = 0,
    Offered = 1,
    Withdrawn = 2
}

export type ProductDto = {
    id: number,
    anonymousSaleAllowed: boolean,
    name: string,
    code: string,
    basePrice: number,
    description?: string,
    status: ProductStatusEnum
}

export type ExtendedProductDto = ProductDto & {
    subProducts: []
}

export type SubProductDto = {
    id: number,
    name: string,
    code: string,
    basePrice: number,
    description?: string,
}