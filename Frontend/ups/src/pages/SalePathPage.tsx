import { Dispatch, useEffect, useReducer } from "react"
import { ExtendedProductDto, AnsweredParameterDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastError } from "../helpers/ToastHelpers"
import { ChooseProductStep } from "../components/salepath/ChooseProductStep"
import { FillClientInfoStep } from "../components/salepath/FillClientInfoStep"
import { ChooseSubProductsStep } from "../components/salepath/ChooseSubProductsStep"
import { FillParameterAnswersStep } from "../components/salepath/FillParameterAnswersStep"
import { SummaryStep } from "../components/salepath/SummaryStep"
import { FinishSaleStep } from "../components/salepath/FinishSaleStep"

enum SalePathStep {
    ChooseProduct = 1,
    FillClientInfo = 2,
    ChooseSubProducts = 3,
    FillParameterAnswers = 4,
    Summary = 5,
    FinishSale = 6
}

export type SalePathAction =
    | { type: 'setProduct', productId: number }
    | { type: 'fetchedProduct', product: ExtendedProductDto }
    | { type: 'nextStep' }
    | { type: 'prevStep' }
    | { type: 'addSubProduct', subProductId: number }
    | { type: 'removeSubProduct', subProductId: number }
    | { type: 'setClient', clientId: number }
    | { type: 'filledParameters', productValues: AnsweredParameterDto[], subProductValues: AnsweredParameterDto[] }

export const getSelectedSubProducts = (state: SalePathState) => {
    return state.product?.subProducts.filter(sp => state.subProductIds.includes(sp.id)) ?? [];
}

type SalePathState = {
    step: SalePathStep
    product: ExtendedProductDto | null,
    productId: number | null,
    subProductIds: number[],
    clientId: number | null,
    productAnswers: AnsweredParameterDto[] | null,
    subProductAnswers: AnsweredParameterDto[] | null,
}

const initalState: SalePathState = {
    product: null,
    productId: null,
    step: SalePathStep.ChooseProduct,
    subProductIds: [],
    clientId: null,
    productAnswers: null,
    subProductAnswers: null,
}

function reducer(state: SalePathState, action: SalePathAction): SalePathState {
    switch(action.type){
        case 'setClient':
            return { ...state, clientId: action.clientId }
        case 'addSubProduct':
            return { ...state, subProductIds: [...state.subProductIds, action.subProductId], subProductAnswers: [] }
        case 'removeSubProduct':
            return { ...state, subProductIds: state.subProductIds.filter(s => s !== action.subProductId), subProductAnswers: [] }
        case 'setProduct':
            return { ...state, productId: action.productId, subProductIds: [], productAnswers: [], product: null, subProductAnswers: [] };
        case 'nextStep':
            return { ...state, step: state.step + 1}
        case 'prevStep':
            let decrement = state.step === SalePathStep.FillParameterAnswers && state.product?.subProducts.length === 0 ?
                2 : 1;
            return { ...state, step: state.step - decrement }
        case 'fetchedProduct':
            return { ...state, product: action.product }
        case 'filledParameters':
            return { ...state, productAnswers: action.productValues, subProductAnswers: action.subProductValues }
    }
}

export type SalePathStepProps = {
    state: SalePathState,
    dispatch: Dispatch<SalePathAction>
}

export default function SalePathPage() {
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
                return !state.clientId && state.product?.anonymousSaleAllowed === false
            case SalePathStep.FillParameterAnswers:
                return (state.productAnswers == null || answeredProductParams.some(pv => !pv.answer && pv.required)) ||
                    (state.subProductAnswers == null || answeredSubProductsParams.some(pv => !pv.answer && pv.required))
        }
        return false
    }

    const answeredProductParams: AnsweredParameterDto[] = !!state.productAnswers && state.productAnswers.length > 0 ?
        state.productAnswers :
        state.product?.parameters.map(p => ({ ...p, answer: undefined })) ?? [];
    const answeredSubProductsParams: AnsweredParameterDto[] = !!state.subProductAnswers && state.subProductAnswers.length > 0 ?
        state.subProductAnswers :
        getSelectedSubProducts(state).flatMap(sp => sp.parameters.map(p => ({ ...p, answer: undefined, subProductId: sp.id }))) ?? []

    return <>
        <h2>Ścieżka Sprzedaży - Krok {state.step.toString()}</h2>
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
        {state.step === SalePathStep.ChooseProduct && <ChooseProductStep  state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.FillClientInfo && <FillClientInfoStep  state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.ChooseSubProducts && <ChooseSubProductsStep state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.FillParameterAnswers && <FillParameterAnswersStep state={state} dispatch={dispatch}
            paramsFormSubProducts={answeredSubProductsParams} paramsFromProduct={answeredProductParams} nextStepDisabled={nextStepDisabled()}
        />}
        {state.step === SalePathStep.Summary && <SummaryStep state={state} dispatch={dispatch} />}
        {state.step === SalePathStep.FinishSale && <FinishSaleStep state={state} dispatch={dispatch} />}
    </>
}