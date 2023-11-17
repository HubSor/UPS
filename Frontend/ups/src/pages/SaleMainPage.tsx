import { Dispatch, useEffect, useReducer } from "react"
import { ExtendedProductDto } from "../api/Dtos"
import { ChooseProductForm } from "../components/salepath/ChooseProductForm"
import { Api } from "../api/Api"
import { toastError } from "../helpers/ToastHelpers"

enum SalePathStep {
    ChooseProduct = 1,
    FillClientInfo = 2, // optional
    ChooseSubProducts = 3,
    FillParameterValues = 4,
    Summary = 5,
    FinishSale = 6
}

export default function SalePathPage() {
    return <SalePathPageInner/>
}

export type SalePathAction =
    | { type: 'setProduct', productId: number }
    | { type: 'fetchedProduct', product: ExtendedProductDto }
    | { type: 'nextStep' }
    | { type: 'prevStep' }

type SalePathState = {
    step: SalePathStep
    product: ExtendedProductDto | null,
    productId: number | null,
}

const initalState: SalePathState = {
    product: null,
    productId: null,
    step: SalePathStep.ChooseProduct,
}

function reducer(state: SalePathState, action: SalePathAction): SalePathState {
    switch(action.type){
        case 'setProduct':
            return { ...state, productId: action.productId };
        case 'nextStep':
            return { ...state, step: state.step + 1}
        case 'prevStep':
            return { ...state, step: state.step - 1 }
        case 'fetchedProduct':
            return { ...state, product: action.product }
    }
}

export type SalePathFormProps = {
    state: SalePathState,
    dispatch: Dispatch<SalePathAction>
}

function SalePathPageInner() {
    const [state, dispatch] = useReducer(reducer, initalState);

    useEffect(() => {
        if (state.productId != null &&
            state.product?.id !== state.productId &&
            state.step > SalePathStep.ChooseProduct)
        {
            Api.GetProduct({ productId: state.productId }).then(res => {
                if (res.data && res.success)
                    dispatch({ type: 'fetchedProduct', product: res.data.product })
                else
                    toastError("Nie udało się pobrać produktu")
            });
        }
    }, [state.product?.id, state.productId, state.step])

    const nextStepDisabled = () => { 
        switch (state.step){
            case SalePathStep.ChooseProduct:
                return !state.productId
        }
    }

    return <>
        <h4>Ścieżka Sprzedaży</h4>
        <strong>Krok {state.step.toString()}</strong>
        <br/>
        <br/>
        {state.step === SalePathStep.ChooseProduct && <ChooseProductForm  state={state} dispatch={dispatch} />}
        <div className="salepath-buttons">
            <div>
                {state.step > SalePathStep.ChooseProduct && <button type="button" className="btn btn-lg btn-outline-primary"
                    onClick={() => {
                        dispatch({ type: 'prevStep' })
                    }}
                >
                    Cofnij
                </button>}
            </div>
            <div>
                {state.step < SalePathStep.Summary && <button type="button" className="btn btn-lg btn-primary"
                    onClick={() => {
                        dispatch({ type: 'nextStep' })
                    }}
                    disabled={nextStepDisabled()}
                >
                    Kontynuuj
                </button>}
            </div>
        </div>
    </>
}