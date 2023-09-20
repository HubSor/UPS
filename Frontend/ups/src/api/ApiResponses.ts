import { UserDto } from "./Dtos"

export type ApiResponse<T> = {
    statusCode: number,
    data?: T,
    errors: {[key: string]: string[]},
    success: boolean,
}

export type LoginResponse = {
    userDto: UserDto
}