import { ParameterTypeEnum, ProductStatusEnum, RoleEnum } from "../api/Dtos";

export const GetRoleDisplayName = (role: RoleEnum) => {
    switch (role) {
        case RoleEnum.Administrator:
            return "Administrator";
        case RoleEnum.UserManager:
            return "Zarządca użytkowników";
        case RoleEnum.Seller:
            return "Sprzedawca";
        case RoleEnum.ProductManager:
            return "Zarządca produktów";
    }
}

export const GetProductStatusDisplayName = (role?: ProductStatusEnum) => {
    switch (role) {
        case ProductStatusEnum.NotOffered:
            return "Nieoferowany";
        case ProductStatusEnum.Withdrawn:
            return "Wycofany";
        case ProductStatusEnum.Offered:
            return "Oferowany";
        default:
            return ""
    }
}

export const GetParameterTypeDisplayName = (type?: ParameterTypeEnum) => {
    switch (type) {
        case ParameterTypeEnum.Integer:
            return "Liczba całkowita";
        case ParameterTypeEnum.Checkbox:
            return "Flaga";
        case ParameterTypeEnum.Decimal:
            return "Liczba dziesiętna";
        case ParameterTypeEnum.Select:
            return "Wybór jednokrotny";
        case ParameterTypeEnum.Text:
            return "Tekst";
        case ParameterTypeEnum.TextArea:
            return "Długi tekst";
        default:
            return ""
    }
}