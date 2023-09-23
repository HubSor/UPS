import secureLocalStorage from "react-secure-storage";
import { UserDto } from "../api/Dtos";

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

    public static HasRole(role: string) {
        return AuthHelpers.GetUserData()?.roles.includes(role);
    }

    public static ClearAllData() {
        secureLocalStorage.clear();
    }
}