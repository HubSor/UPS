export type UserDto = {
    username: string
    roles: string[]
}

export enum RoleEnum {
    Administrator = 0,
    Seller = 1,
    UserManager = 2
} 