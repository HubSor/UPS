import { CompanyClientDto, ExtendedProductDto, ExtendedSubProductDto, PagedList, PersonClientDto, ProductDto, SaleDto, SubProductDto, UserDto } from "./Dtos"

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

export type UpsertClientResponse = {
    clientId: number
}

export type FindPersonClientResponse = {
    personClient: PersonClientDto
}

export type FindCompanyClientResponse = {
    companyClient: CompanyClientDto
}

export type ListSalesResponse = {
    sales: PagedList<SaleDto>
}

export type ListPersonClientsResponse = {
    clients: PagedList<PersonClientDto>
}

export type ListCompanyClientsResponse = {
    clients: PagedList<CompanyClientDto>
}