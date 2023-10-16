import { PaginationDto } from "./Dtos"

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