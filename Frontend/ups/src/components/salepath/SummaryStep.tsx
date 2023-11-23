import { SalePathStepProps, getSelectedSubProducts } from "../../pages/SalePathPage"
import { DisplayInfoRow } from "../products/DisplayInfoRow"
import React from "react"
import { DisplayParameterRow } from "../parameters/DisplayParameterRow"
import { Button } from "react-bootstrap"

type SummaryProps = SalePathStepProps

export const SummaryStep = ({ state, dispatch }: SummaryProps) => {
    const selectedSubProducts = getSelectedSubProducts(state)
    const initialPrice = selectedSubProducts.reduce((agg, curr) => agg + curr.price, 0) +
        (state.product?.basePrice ?? 0)
    const basePrice = selectedSubProducts.reduce((agg, curr) => agg + curr.basePrice, 0) +
        (state.product?.basePrice ?? 0)

    const handleSubmit = () => {

    }

    return <div>
        <h3>Podsumowanie sprzedaży</h3>
        <div className="alert alert-primary">
            Upewnij się, że dane sprzedaży są poprawne.<br/>
            Po zatwierdzeniu tego formularza, sprzedaż będzie zarejestrowana w systemie.
        </div>
        <h5 className="align-left">
            Dane produktu
        </h5>
        <div className="card">
            <div className="card-body product-info">
                <DisplayInfoRow value={state.product?.code} name="Kod" />
                <DisplayInfoRow value={state.product?.name} name="Nazwa" />
                <DisplayInfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena" />
                <DisplayInfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż" />
            </div>
        </div>
        <div className="list-group">
            {state.productAnswers?.map(p => <DisplayParameterRow parameter={p} key={p.id} /> )}
        </div>
        <hr />
        {selectedSubProducts.length > 0 && <h5 className="align-left">
            Dane podproduktów
        </h5>}
        {selectedSubProducts.map(sp => {
            return <React.Fragment key={sp.id}>
                <div className="card">
                    <div className="card-body product-info">
                        <DisplayInfoRow value={sp.code} name="Kod" />
                        <DisplayInfoRow value={sp.name} name="Nazwa" />
                        <DisplayInfoRow value={sp.price.toString()} name="Cena w wybranym produkcie" />
                        <DisplayInfoRow value={sp.basePrice.toString()} name="Podstawowa cena" />
                    </div>
                </div>
                <div className="list-group">
                    {state.subProductAnswers?.filter(sppv => sppv.subProductId === sp.id && !!sppv.answer)?.map(p =>
                        <DisplayParameterRow parameter={p} key={p.id} />
                    )}
                </div>
                <br/>
            </React.Fragment>
        })}
        <hr/>
        <div className="form-group row justify-content-center mb-2">
            <label className="col-sm-3 col-form-label">Suma podstawowych cen</label>
            <div className="col-sm-2">
                <input disabled type="number" className="form-control" defaultValue={basePrice} />
            </div>
        </div>
        <div className="form-group row justify-content-center">
            <strong className="col-sm-3 col-form-label">Całkowita kwota do zapłaty</strong>
            <div className="col-sm-2">
                <input type="number" className="form-control" defaultValue={initialPrice}/>
            </div>
        </div>
        <br/>
        <div>
            <Button type="button" size="lg" onClick={handleSubmit}>
                Zatwierdź sprzedaż
            </Button>
        </div>
    </div>
}