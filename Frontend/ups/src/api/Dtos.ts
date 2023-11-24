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

export const GetRoleKey = (role: RoleEnum): string => {
    return RoleEnum[role];
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
    subProducts: ExtendedSubProductDto[]
    parameters: ParameterDto[]
}

export type SubProductDto = {
    id: number,
    name: string,
    code: string,
    basePrice: number,
    description?: string,
}

export type ExtendedSubProductDto = SubProductDto & {
    price: number,
    parameters: ParameterDto[],
    products: ProductDto[],
}

export enum ParameterTypeEnum {
    Text = 0,
    Integer = 1,
    Decimal = 2,
    Select = 3,
    Checkbox = 4,
    TextArea = 5
}

export type OptionDto = {
    value: string
}

export type ExtendedOptionDto = OptionDto & {
    id: number,
    parameterId: number
}

export type ParameterDto = {
    id: number,
    name: string,
    type: ParameterTypeEnum,
    required: boolean,
    options: ExtendedOptionDto[]
}

export type AnsweredParameterDto = ParameterDto & {
    answer?: string,
    subProductId?: number
}

export type SaveSaleParameterDto = {
    parameterId: number,
    answer?: string
}

export type ClientDto = {
    phoneNumber?: string,
    email?: string,
    id: number
}

export type CompanyClientDto = ClientDto & {
    companyName: string,
    regon?: string,
    nip?: string
}

export type PersonClientDto = ClientDto & {
    firstName: string,
    lastName: string,
    pesel?: string
}

export type SaleDto = {
    saleId: number,
    productCode: string,
    subProductCodes: string[],
    personClient?: PersonClientDto,
    companyClient?: CompanyClientDto,
    saleTime: string,
    totalPrice: number,
}