import { Form } from "react-bootstrap"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { InfoRow } from "../InfoRow"
import React from "react"

type SummaryProps = SalePathFormProps

export const SummaryForm = ({ state, dispatch }: SummaryProps) => {
    return <div>
        <h3>Podsumowanie sprzedaży</h3>
        <Form.Label className="align-left">
            Dane produktu
        </Form.Label>
        <div className="card">
            <form className="card-body product-info">
                <InfoRow value={state.product?.code} name="Kod" />
                <InfoRow value={state.product?.name} name="Nazwa" />
                <InfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena" />
                <InfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż" />
            </form>
        </div>
        {
            ""
            // zmiana ceny produktu
            // parametry produktu
        }
        <Form.Label className="align-left">
            Dane podproduktów
        </Form.Label>
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
                <br/>
                {
                    ""
                    // zmiana ceny
                    // parametry podproduktu
                }
            </React.Fragment>
        })}
    </div>
}