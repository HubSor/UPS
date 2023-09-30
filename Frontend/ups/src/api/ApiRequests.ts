export type LoginRequest = {
    username: string,
    password: string
}

export type AddUserRequest = {
    username: string,
    password: string,
    roleIds: number[]
}