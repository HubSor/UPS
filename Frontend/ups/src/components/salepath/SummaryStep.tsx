import { SalePathStepProps } from "../../pages/SalePathPage"
import { DisplayInfoRow } from "../products/DisplayInfoRow"
import React from "react"
import { DisplayParameterRow } from "../parameters/DisplayParameterRow"

type SummaryProps = SalePathStepProps

export const SummaryStep = ({ state, dispatch }: SummaryProps) => {
    return <div>
        <h3>Podsumowanie sprzedaży</h3>
        <h5 className="align-left">
            Dane produktu
        </h5>
        <div className="card">
            <form className="card-body product-info">
                <DisplayInfoRow value={state.product?.code} name="Kod" />
                <DisplayInfoRow value={state.product?.name} name="Nazwa" />
                <DisplayInfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena" />
                <DisplayInfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż" />
            </form>
        </div>
        <div className="list-group">
            {state.productAnswers?.map(p => <DisplayParameterRow parameter={p} key={p.id} /> )}
        </div>
        <h5 className="align-left">
            Dane podproduktów
        </h5>
        {state.product?.subProducts.filter(sp => state.subProductIds.includes(sp.id)).map(sp => {
            return <React.Fragment key={sp.id}>
                <div className="card">
                    <form className="card-body product-info">
                        <DisplayInfoRow value={sp.code} name="Kod" />
                        <DisplayInfoRow value={sp.name} name="Nazwa" />
                        <DisplayInfoRow value={sp.price.toString()} name="Cena w wybranym produkcie" />
                        <DisplayInfoRow value={sp.basePrice.toString()} name="Podstawowa cena" />
                    </form>
                </div>
                <div className="list-group">
                    {state.subProductAnswers?.filter(sppv => sppv.subProductId === sp.id)?.map(p =><DisplayParameterRow parameter={p} key={p.id} />)}
                </div>
                <br/>
            </React.Fragment>
        })}
    </div>
}