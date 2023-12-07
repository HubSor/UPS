import { SalePathStepProps, getSelectedSubProducts } from "../../pages/SalePathPage"
import { DisplayInfoRow } from "../products/DisplayInfoRow"
import React, { useReducer, useState } from "react"
import { DisplayParameterRow } from "../parameters/DisplayParameterRow"
import { Button } from "react-bootstrap"
import { Api } from "../../api/Api"
import { AnsweredParameterDto, ExtendedProductDto, ExtendedSubProductDto, ParameterTypeEnum, SaveSaleParameterDto } from "../../api/Dtos"
import { validateDecimal } from "../../helpers/ParameterHelpers"
import { toastDefaultError } from "../../helpers/ToastHelpers"
import { JoinErrors, NoFormikValidationMessage, taxText } from "../../helpers/FormHelpers"

type SummaryProps = SalePathStepProps & {
    product: ExtendedProductDto
}

type SalePathSubProduct = ExtendedSubProductDto & {
    error?: string,
    finalPrice: number
}

type SummaryState = {
    subProducts: SalePathSubProduct[]
}

type SummaryEvent = { type: 'updateError', idx: number, error?: string } |
    { type: 'updatePrice', idx: number, price: number }

const reducer = (state: SummaryState, action: SummaryEvent): SummaryState => {
    switch (action.type) {
        case 'updateError':
            let newProductsWithErrors = state.subProducts;
            newProductsWithErrors.at(action.idx)!.error = action.error;
            return { ...state, subProducts: newProductsWithErrors }
        case 'updatePrice':
            let newProductsWithPrices = state.subProducts;
            newProductsWithPrices.at(action.idx)!.finalPrice = action.price;
            return { ...state, subProducts: newProductsWithPrices }
    }
} 

export const SummaryStep = ({ state, dispatch, product }: SummaryProps) => {
    const selectedSubProducts = getSelectedSubProducts(state)
    const [localState, localDispatch] = useReducer(reducer, { 
        subProducts: selectedSubProducts.map(sp => ({ ...sp, finalPrice: sp.price, error: undefined })) 
    })
    const basePrice = selectedSubProducts.reduce((agg, curr) => agg + curr.basePrice, 0) + product.basePrice

    const [productPrice, setProductPrice] = useState(product.basePrice);
    const [productPriceError, setProductPriceError] = useState<string | undefined>();
    const [submitErrorMsg, setSubmitErrorMsg] = useState<string | undefined>();

    const totalPrice = productPrice + localState.subProducts.reduce((agg, curr) => agg + curr.finalPrice, 0)
    const productTax = productPrice * product.taxRate
    const subProductTax = localState.subProducts.reduce((agg, curr) => agg + curr.finalPrice * curr.taxRate, 0)
    const totalTax = productTax + subProductTax;

    const disabledConfirmButon = !state.productId || !!productPriceError || localState.subProducts.some(e => !!e.error);

    const prepareAnswer = (param: AnsweredParameterDto): SaveSaleParameterDto => {
        const answer = param.type === ParameterTypeEnum.Checkbox ?
            param.answer === true ? "TAK" : "NIE" :
            param.answer?.toString();

        return {
            parameterId: param.id,
            answer: answer
        }
    }

    const handleSubmit = () => {
        const answers: SaveSaleParameterDto[] = [
            ...state.productAnswers?.map(prepareAnswer) ?? [],
            ...state.subProductAnswers?.map(prepareAnswer) ?? []
        ]

        Api.SaveSale({
            productId: state.productId!,
            subProductIds: state.subProductIds,
            clientId: state.clientId ?? undefined,
            totalPrice: totalPrice,
            answers: answers
        }).then(res => {
            if (res.data && res.success){
                dispatch({ type: 'nextStep' })
                setSubmitErrorMsg(undefined)
            }
            else if (res.errors){
                setSubmitErrorMsg(JoinErrors(res.errors))
            }
            else toastDefaultError();
        })
    }

    return <div>
        <h3>Podsumowanie sprzedaży</h3>
        <div className="alert alert-primary">
            Upewnij się, że dane sprzedaży są poprawne.<br/>
            Po zatwierdzeniu tego formularza, sprzedaż będzie zarejestrowana w systemie.
        </div>
        <NoFormikValidationMessage msg={submitErrorMsg} />
        <h5 className="align-left">
            Dane produktu
        </h5>
        <div className="card">
            <div className="card-body product-info">
                <DisplayInfoRow value={product.code} name="Kod" />
                <DisplayInfoRow value={product.name} name="Nazwa" />
                <DisplayInfoRow value={product.basePrice.toString()} name="Podstawowa cena" />
                <DisplayInfoRow value={taxText(product.taxRate)} name="Stawka podatku" />
                <DisplayInfoRow value={product.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż" />
            </div>
        </div>
        <div className="form-group row justify-content-center mt-2 mb-2">
            <label className="col-sm-3 col-form-label">Ostateczna cena produktu</label>
            <div className="col-sm-2">
                <input type="number" className="form-control"
                    onChange={e => {
                        const validation = validateDecimal(e.currentTarget.value);
                        if (!validation) {
                            setProductPriceError(undefined)
                            setProductPrice(+e.currentTarget.value)
                        }
                        else
                            setProductPriceError(validation)
                    }}
                    value={productPrice}
                />
                <NoFormikValidationMessage msg={productPriceError} />
            </div>
        </div>
        <div className="list-group">
            {state.productAnswers?.map(p => <DisplayParameterRow parameter={p} key={p.id} /> )}
        </div>
        <hr />
        {selectedSubProducts.length > 0 && <h5 className="align-left">
            Dane podproduktów
        </h5>}
        {selectedSubProducts.map((sp, idx) => {
            return <React.Fragment key={sp.id}>
                <div className="card">
                    <div className="card-body product-info">
                        <DisplayInfoRow value={sp.code} name="Kod" />
                        <DisplayInfoRow value={sp.name} name="Nazwa" />
                        <DisplayInfoRow value={sp.price.toString()} name="Cena w wybranym produkcie" />
                        <DisplayInfoRow value={sp.basePrice.toString()} name="Podstawowa cena" />
                        <DisplayInfoRow value={taxText(sp.taxRate)} name="Stawka podatku" />
                    </div>
                </div>
                <div className="form-group row justify-content-center mt-2 mb-2">
                    <label className="col-sm-3 col-form-label">Ostateczna cena podproduktu</label>
                    <div className="col-sm-2">
                        <input type="number" className="form-control"
                            onChange={e => {
                                const validation = validateDecimal(e.currentTarget.value);
                                if (!validation) {
                                    localDispatch({ type: 'updateError', idx: idx });
                                    localDispatch({ type: 'updatePrice', idx: idx, price: +e.currentTarget.value });
                                }
                                else
                                    localDispatch({ type: 'updateError', idx: idx, error: validation });
                            }}
                            value={localState.subProducts[idx]?.finalPrice ?? 0.00}
                        />
                        <NoFormikValidationMessage msg={localState.subProducts[idx].error} />
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
                <input disabled className="form-control" value={basePrice.toFixed(2)} />
            </div>
        </div>
        <div className="form-group row justify-content-center mb-2">
            <label className="col-sm-3 col-form-label">Całkowita kwota do zapłaty</label>
            <div className="col-sm-2">
                <input disabled className="form-control" value={totalPrice.toFixed(2)} />
            </div>
        </div>
        <div className="form-group row justify-content-center mb-2">
            <label className="col-sm-3 col-form-label">Całkowita kwota podatku</label>
            <div className="col-sm-2">
                <input disabled className="form-control" value={totalTax.toFixed(2)} />
            </div>
        </div>
        <br/>
        <div>
            <Button type="button" size="lg" onClick={handleSubmit}
                disabled={disabledConfirmButon}
            >
                Zatwierdź sprzedaż
            </Button>
        </div>
    </div>
}