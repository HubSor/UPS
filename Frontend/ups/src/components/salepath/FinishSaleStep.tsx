import { Button } from "react-bootstrap"
import { SalePathStepProps } from "../../pages/SalePathPage"
import { useNavigate } from "react-router-dom"
import { Paths } from "../../App"
import { AuthHelpers } from "../../helpers/AuthHelper"
import { RoleEnum } from "../../api/Dtos"

type FinishSaleProps = SalePathStepProps

export const FinishSaleStep = ({ state, dispatch }: FinishSaleProps) => {
    const nav = useNavigate();

    return <div>
        <h3>Sprzedaż zakończona</h3>
        <br/>
        <div>
            Sprzedaż została pomyślnie zarejestrowana.
        </div>
        {AuthHelpers.HasAnyRole([RoleEnum.Administrator, RoleEnum.SaleManager]) && <Button
            type="button" className="m-2" onClick={() => nav(Paths.sales)}
        >
            Przejdź do listy transakcji 
        </Button>}
    </div>
}