import { SalePathFormProps } from "../../pages/SaleMainPage"
import { InfoRow } from "../InfoRow"
import React from "react"
import { DisplayParameterRow } from "../../helpers/ParameterHelpers"

type SummaryProps = SalePathFormProps

export const SummaryForm = ({ state, dispatch }: SummaryProps) => {
    return <div>
        <h3>Podsumowanie sprzedaży</h3>
        <h5 className="align-left">
            Dane produktu
        </h5>
        <div className="card">
            <form className="card-body product-info">
                <InfoRow value={state.product?.code} name="Kod" />
                <InfoRow value={state.product?.name} name="Nazwa" />
                <InfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena" />
                <InfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż" />
            </form>
        </div>
        <div className="list-group">
            {state.productParameterValues?.map(p => <DisplayParameterRow parameter={p} key={p.id} /> )}
        </div>
        <h5 className="align-left">
            Dane podproduktów
        </h5>
        {state.product?.subProducts.filter(sp => state.subProductIds.includes(sp.id)).map(sp => {
            return <React.Fragment key={sp.id}>
                <div className="card">
                    <form className="card-body product-info">
                        <InfoRow value={sp.code} name="Kod" />
                        <InfoRow value={sp.name} name="Nazwa" />
                        <InfoRow value={sp.price.toString()} name="Cena w wybranym produkcie" />
                        <InfoRow value={sp.basePrice.toString()} name="Podstawowa cena" />
                    </form>
                </div>
                <div className="list-group">
                    {state.subProductParameterValues?.filter(sppv => sppv.subProductId === sp.id)?.map(p =><DisplayParameterRow parameter={p} key={p.id} />)}
                </div>
                <br/>
            </React.Fragment>
        })}
    </div>
}