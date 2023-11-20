import { Dispatch, useEffect, useReducer } from "react"
import { ExtendedProductDto, SalePathParameterDto } from "../api/Dtos"
import { ChooseProductForm } from "../components/salepath/ChooseProductForm"
import { Api } from "../api/Api"
import { toastError } from "../helpers/ToastHelpers"
import { ChooseSubProductsForm } from "../components/salepath/ChooseSubProductsForm"
import { FillClientInfoForm } from "../components/salepath/FillClientInfoForm"
import { FillParameterValuesForm } from "../components/salepath/FillParameterValuesForm"
import { SummaryForm } from "../components/salepath/SummaryForm"

enum SalePathStep {
    ChooseProduct = 1,
    FillClientInfo = 2,
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
    | { type: 'addSubProduct', subProductId: number }
    | { type: 'removeSubProduct', subProductId: number }
    | { type: 'setClient', clientId: number }
    | { type: 'filledParameters', paramterValues: SalePathParameterDto[] }

type SalePathState = {
    step: SalePathStep
    product: ExtendedProductDto | null,
    productId: number | null,
    subProductIds: number[],
    clientId: number | null,
    parameterValues: SalePathParameterDto[] | null,
}

const initalState: SalePathState = {
    product: null,
    productId: null,
    step: SalePathStep.ChooseProduct,
    subProductIds: [],
    clientId: null,
    parameterValues: null,
}

function reducer(state: SalePathState, action: SalePathAction): SalePathState {
    switch(action.type){
        case 'setClient':
            return { ...state, clientId: action.clientId }
        case 'addSubProduct':
            return { ...state, subProductIds: [ ...state.subProductIds, action.subProductId ]}
        case 'removeSubProduct':
            return { ...state, subProductIds: state.subProductIds.filter(s => s !== action.subProductId)}
        case 'setProduct':
            return { ...state, productId: action.productId, subProductIds: [], parameterValues: [], product: null };
        case 'nextStep':
            return { ...state, step: state.step + 1}
        case 'prevStep':
            let decrement = state.step === SalePathStep.FillParameterValues && state.product?.subProducts.length === 0 ?
                2 : 1;
            return { ...state, step: state.step - decrement }
        case 'fetchedProduct':
            return { ...state, product: action.product }
        case 'filledParameters':
            return { ...state, parameterValues: action.paramterValues }
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

    useEffect(() => {
        if (state.product?.id === state.productId && !!state.product ){
            if (state.step === SalePathStep.ChooseSubProducts && state.product.subProducts.length === 0)
                dispatch({ type: 'nextStep' })
        }

    }, [state.step, state.product, state.productId])

    const nextStepDisabled = () => { 
        switch (state.step){
            case SalePathStep.ChooseProduct:
                return !state.productId
            case SalePathStep.FillClientInfo:
                return (!state.clientId && state.product?.anonymousSaleAllowed === false)
            case SalePathStep.FillParameterValues:
                return (state.parameterValues == null || state.parameterValues.some(pv => !pv.answer && pv.required))
        }
    }

    return <>
        <h4>Ścieżka Sprzedaży</h4>
        <strong>Krok {state.step.toString()}</strong>
        <br/>
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
        {state.step === SalePathStep.ChooseProduct && <ChooseProductForm  state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.FillClientInfo && <FillClientInfoForm  state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.ChooseSubProducts && <ChooseSubProductsForm state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.FillParameterValues && <FillParameterValuesForm state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.Summary && <SummaryForm state={state} dispatch={dispatch} />}
    </>
}