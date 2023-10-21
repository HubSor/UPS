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