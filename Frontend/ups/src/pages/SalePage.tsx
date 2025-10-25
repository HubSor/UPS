import React, { useEffect, useReducer } from "react"
import { SaleDetailsDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useNavigate, useParams } from "react-router-dom"
import { Form } from "react-bootstrap"
import { DisplayInfoRow } from "../components/products/DisplayInfoRow"
import { DisplayParameterRow } from "../components/parameters/DisplayParameterRow"

export default function SalePage() {
    const { id } = useParams();

    return  <>
        {!!id && <SalePageInner saleId={+id} />}
    </>
}

type SalePageState = {
    sale: SaleDetailsDto | null,
    refresh: boolean
}

const initalState: SalePageState = {
    sale: null,
    refresh: true,
}

type SalePageAction =
    | { type: 'refresh' }
    | { type: 'fetched', sale: SaleDetailsDto }

function reducer(state: SalePageState, action: SalePageAction): SalePageState {
    switch (action.type) {
        case 'refresh':
            return { ...state, refresh: true }
        case 'fetched':
            return { ...state, sale: action.sale, refresh: false }
    }
}

type SalePageProps = {
    saleId: number
}

function SalePageInner({ saleId }: SalePageProps) {
    const [state, dispatch] = useReducer(reducer, initalState);
    const nav = useNavigate();

    useEffect(() => {
        const fetchSale = () => {
            Api.GetSale({ saleId: saleId }).then(res => {
                if (res.success && res.data) {
                    dispatch({ type: 'fetched', sale: res.data.saleDetailsDto })
                }
                else toastDefaultError();
            })
        }

        if (state.refresh)
            fetchSale();
    }, [saleId, state.refresh])

    const subProductTax = (state.sale?.totalTax ?? 0) - (state.sale?.productTax ?? 0);

    return <>
        <h3>Transakcja {saleId}</h3>
        <br />
        <Form.Label className="align-left">
            Dane transakcji
        </Form.Label>
        <div className="card">
            <form className="card-body product-info">
                <DisplayInfoRow value={state.sale?.totalPrice.toFixed(2).replace('.', ',')} name="Całkowita należność klienta" />
                <DisplayInfoRow value={state.sale?.totalTax.toFixed(2).replace('.', ',')} name="Całkowity podatek" />
                <DisplayInfoRow value={state.sale?.productPrice.toFixed(2).replace('.', ',')} name="Ostateczna cena produktu" />
                <DisplayInfoRow value={state.sale?.productTax.toFixed(2).replace('.', ',')} name="Podatek na produkcie" />
                <DisplayInfoRow value={state.sale?.saleTime} name="Data rejestracji" />
                <DisplayInfoRow value={state.sale?.productCode} name="Kod produktu" />
                <DisplayInfoRow value={state.sale?.clientId?.toString()} name="Id klienta" />
                <DisplayInfoRow value={state.sale?.clientName} name="Klient" />
                <DisplayInfoRow value={state.sale?.subProductCodes} name="Wybrane podprodukty" />
                <DisplayInfoRow value={subProductTax?.toFixed(2).replace('.', ',')} name="Podatek na podproduktach" />
            </form>
        </div>
        <Form.Label className="align-left">
            Uzupełnione parametry
        </Form.Label>
        <div className="list-group">
            {state.sale?.parameters?.map(p => <DisplayParameterRow parameter={p} key={p.id} />)}
        </div>
    </>
}
