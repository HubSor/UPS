import secureLocalStorage from "react-secure-storage";
import { GetRoleKey, RoleEnum, UserDto } from "../api/Dtos";

export class AuthHelpers {
    private static userKey = "userData";

    public static StoreUserData(userData: UserDto) {
        secureLocalStorage.setItem(this.userKey, userData);
    }

    public static GetUserData(): UserDto | undefined {
        return secureLocalStorage.getItem(this.userKey) as UserDto;
    }

    public static IsLoggedIn(): boolean {
        return !!AuthHelpers.GetUserData();
    }

    public static HasRole(role: RoleEnum) {
        return AuthHelpers.HasAnyRole([role]);
    }

    public static HasAnyRole(roles: RoleEnum[]){
        const userData = AuthHelpers.GetUserData()?.roles;
        return roles.some(role => userData?.includes(GetRoleKey(role)));
    }

    public static HasUserRoles(){
        return AuthHelpers.HasAnyRole([RoleEnum.Administrator, RoleEnum.UserManager]);
    }

    public static HasProductRoles() {
        return AuthHelpers.HasAnyRole([RoleEnum.Administrator, RoleEnum.ProductManager]);
    }

    public static HasSalesRoles() {
        return AuthHelpers.HasAnyRole([RoleEnum.Administrator, RoleEnum.Seller]);
    }

    public static ClearAllData() {
        secureLocalStorage.clear();
    }
}