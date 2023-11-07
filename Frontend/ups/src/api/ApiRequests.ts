import { OptionDto, PaginationDto, ParameterTypeEnum, ProductStatusEnum } from "./Dtos"

export type LoginRequest = {
    username: string,
    password: string
}

export type AddUserRequest = {
    username: string,
    password: string,
    roleIds: number[]
}

export type ListUsersRequest = {
    pagination: PaginationDto
}

export type EditUserRequest = {
    username: string,
    password?: string,
    roleIds: number[]
    id: number,
}

export type DeleteUserRequest = {
    id: number
}

export type AddProductRequest = {
    anonymousSaleAllowed: boolean,
    code: string,
    name: string,
    basePrice: number,
    description?: string
}

export type AddSubProductRequest = {
    code: string,
    name: string,
    basePrice: number,
    description?: string
    productId?: number
}

export type AssignSubProductRequest = {
    productId: number,
    subProductId: number,
    price: number
}

export type UnassignSubProductsRequest = {
    productId: number,
    subProductIds: number[]
}

export type ListProductsRequest = {
    statuses: ProductStatusEnum[],
    pagination: PaginationDto
}

export type ListSubProductsRequest = {
    productId?: number,
    pagination: PaginationDto
}

export type EditProductRequest = {
    anonymousSaleAllowed: boolean,
    code: string,
    name: string,
    basePrice: number,
    description?: string,
    id: number,
    status: ProductStatusEnum
}

export type EditSubProductRequest = {
    code: string,
    name: string,
    basePrice: number,
    description?: string,
    id: number,
}

export type EditSubProductAssignmentRequest = {
    productId: number,
    subProductId: number,
    newPrice: number
}

export type DeleteProductRequest = {
    productId: number
}

export type DeleteSubProductRequest = {
    subProductId: number
}

export type GetProductRequest = {
    productId: number
}

export type AddParameterRequest = {
    name: string,
    required: boolean,
    type: ParameterTypeEnum,
    productId?: number,
    subProductId?: number,
    options: OptionDto[]
}

export type EditParameterRequest = {
    parameterId: number,
    name: string,
    required: boolean,
    type: ParameterTypeEnum,
    options: OptionDto[]
}

export type DeleteParameterRequest = {
    parameterId: number
}

export type AddOptionRequest = {
    parameterId: number,
    value: string
}

export type DeleteOptionRequest = {
    optionId: number
}