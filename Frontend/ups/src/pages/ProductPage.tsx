import { useCallback, useEffect, useReducer } from "react"
import { ExtendedProductDto, ResultPaginationDto, SubProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"

export default function ProductPage() {
    const { id } = useParams();

    return  <>
        {!!id && <ProductPageInner productId={+id} />}
    </>
}

type ProductPageState = {
    product: ExtendedProductDto | null,
    otherSubProducts: SubProductDto[]
    refresh: boolean
    subProductsPagination: ResultPaginationDto
    otherSubProductsPagination: ResultPaginationDto
    showOtherSubProducts: boolean
}

const defaultPagination: ResultPaginationDto = {
    pageSize: 10,
    pageIndex: 0,
    totalCount: 0,
    totalPages: 1,
    count: 0
}

const initalState: ProductPageState = {
    product: null,
    refresh: true,
    otherSubProducts: [],
    subProductsPagination: defaultPagination,
    otherSubProductsPagination: defaultPagination,
    showOtherSubProducts: false
}

type ProductPageAction =
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'fetchedSubProducts', subProducts: SubProductDto[], pagination: ResultPaginationDto }
    | { type: 'fetchedProduct', product: ExtendedProductDto }

function reducer(state: ProductPageState, action: ProductPageAction): ProductPageState {
    switch (action.type) {
        case 'closeModal':
            return { ...state, }
        case 'refresh':
            return { ...state, refresh: true }
        case 'fetchedSubProducts':
            return { 
                ...state,
                refresh: false, 
                otherSubProducts: action.subProducts,
                otherSubProductsPagination: action.pagination 
            }
        case 'fetchedProduct':
            return { ...state, product: action.product, refresh: false }
    }
}

type ProductPageProps = {
    productId: number
}

export function ProductPageInner({ productId }: ProductPageProps) {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchOtherSubProduts = useCallback(() => {
        Api.ListSubProducts({ pagination: state.otherSubProductsPagination, productId: productId }).then(res => {
            if (res.success && res.data) {
                dispatch({ type: 'fetchedSubProducts', subProducts: res.data.subProducts.items, pagination: res.data.subProducts.pagination })
            }
            else toastDefaultError();
        })
    }, [productId, state.otherSubProductsPagination])

    const fetchProduct = useCallback(() => {
        Api.GetProduct({ productId: productId }).then(res => {
            if (res.success && res.data) {
                dispatch({ type: 'fetchedProduct', product: res.data.product })
            }
            else toastDefaultError();
        })
    }, [productId])

    useEffect(() => {
        if (state.refresh)
            fetchProduct();
    }, [productId, state.refresh, fetchProduct])

    useEffect(() => {
        if (state.refresh && state.showOtherSubProducts)
            fetchOtherSubProduts();
    }, [state.showOtherSubProducts, state.refresh, fetchOtherSubProduts])

    return <>
        <h3>{state.product?.code} {state.product?.name}</h3>
        <br />

    </>
}
