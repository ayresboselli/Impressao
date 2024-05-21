# import win32serviceutil
# import win32service
# import win32event
# import servicemanager
# import socket

import psycopg2.extras
import os, sys
import shutil
import barcode
import img2pdf

from barcode.writer import ImageWriter
from PIL import Image, ImageDraw, ImageFont
from dotenv import load_dotenv

debug = False
path_root = os.path.dirname(os.path.realpath(__file__))

dotenv_path = path_root+'/.env'
load_dotenv(dotenv_path)

Image.LOAD_TRUNCATED_IMAGES = True
dpm = 11.811023622047244 # densidade por milimetro


connection = psycopg2.connect(
    host="localhost",
    user="postgres",
    password="1234",
    database="impressao",
)
cursor = connection.cursor(cursor_factory=psycopg2.extras.RealDictCursor)

cursor.execute('SELECT * FROM public."Config"')
config = cursor.fetchone()

font_path = 'C:/Program Files/Piovelli/backend/sans-serif.ttf'
font = ImageFont.truetype(font_path, int(config['TamanhoFonte']))



##### IMPORTAÇÃO #####
class Grupo:
    def __init__(self,Count,Largura,Altura):
        self.Count = Count
        self.Largura = Largura
        self.Altura = Altura


def SalvarImagem(produto, img, icc_profile, filename):
    global config
    global dpm

    try:
        prodLargura = int(produto['Largura'] * dpm)
        prodAltura = int(produto['Altura'] * dpm)
        prodVertical = produto['Largura'] < produto['Altura']

        # Orientação do arquivo
        imgVertical = img.size[0] < img.size[1]

        if prodVertical != imgVertical:
            img = img.transpose(Image.ROTATE_90)


        # Redimensionar
        if prodLargura != img.size[0] or prodAltura != img.size[1]:
            img = img.resize((int(prodLargura), int(prodAltura)), Image.LANCZOS)

        img.save(config['PathFotos'] + str(item['PedidoId']) + "\\" + str(item['Id']) + "\\" + filename, 'JPEG', dpi=(config['DPI'], config['DPI']), icc_profile=icc_profile)

        thumbLarg = 600 * 100
        thumbAlt = int(thumbLarg / img.size[0] * img.size[1])
        img.thumbnail((thumbLarg, thumbAlt))
        
        img.save(config['WwwPath'] + "/img/thumbs/" + filename, 'JPEG')
        
    except:
        None
    finally:
        img = None
    

def Index(item):
    global config
    global cursor
    global dpm
    global font

    try:
        cursor.execute('SELECT * FROM public."Produto" WHERE "Id" = '+str(item['ProdutoId']))
        produto = cursor.fetchone()
        
        cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id']))
        pedidoItemUploads = cursor.fetchall()


        albuns = []
        for arq in pedidoItemUploads:
            if not arq['Album'] in albuns:
                albuns.append(arq['Album'])


        # arquivos por página
        largPag = float(produto['LarguraMidia']) - float(config['MargemMidia'])
        altPag = float(produto['AlturaMidia']) - float(config['MargemMidia'])
        largThumb = float(config['LarguraThumb']) + float(config['MargemThumb']) * 2
        altThumb = float(config['AlturaThumb']) + float(config['MargemThumb']) * 2
        thumbPorPag = int((largPag / largThumb) * (altPag / (altThumb + float(config['TamanhoFonte']))))

        for alb in albuns:
            # remove arquivos de index antigos


            # arquivos por album
            cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id'])+' AND "Album" = \''+str(alb)+'\' ORDER BY "NomeArquivo"')
            arqList = cursor.fetchall()
            
            cntArq = 0
            cntPag = 0
            while cntArq < len(arqList):
                midia = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

                # criar o thumb
                cntArqCol = 0
                cntArqPag = 0
                posX = int(config['MargemMidia'])
                posY = int(config['MargemMidia'])
                while cntArqPag < thumbPorPag and cntArq < len(arqList):
                    img = Image.open(config['PathFotos'] + str(item['PedidoId']) + "/" + str(item['Id']) + "/" + arqList[cntArq]['Path'])
                    thumb = Image.new('RGB', (int(largThumb*dpm), int(altThumb*dpm)), (255, 255, 255))

                    imgLarg = 0
                    imgAlt = 0
                    tPosX = 0
                    tPosY = 0

                    if img.size[0] > img.size[1]:
                        imgLarg = int(config['LarguraThumb'] * dpm)
                        imgAlt = int((img.size[1] / img.size[0]) * (config['AlturaThumb'] * dpm))
                    else:
                        imgLarg = int((img.size[0] / img.size[1]) * (config['LarguraThumb'] * dpm))
                        imgAlt = int(config['AlturaThumb'] * dpm)


                    tPosX = int(((largThumb * dpm) - imgLarg) / 2)
                    tPosY = int(((altThumb * dpm) - imgAlt) / 2)

                    thumbImg = img.resize((imgLarg, imgAlt))
                    thumb.paste(thumbImg, (tPosX, tPosY))

                    # Texto no thumbnail
                    draw = ImageDraw.Draw(thumb)
                    textsize = draw.textbbox((0,0), arqList[cntArq]['NomeArquivo'], font)
                    txt_width = textsize[2] - textsize[0] + 10
                    txt_height = textsize[3] - textsize[1] + 15

                    img_inf = Image.new('RGB', (txt_width, txt_height), (255, 255, 255))
                    draw = ImageDraw.Draw(img_inf)
                    draw.text((5, 0), arqList[cntArq]['NomeArquivo'], (0,0,0), font=font)
                    
                    # salva texto na imagem
                    inf_w = int(((largThumb * dpm) - txt_width) / 2)
                    inf_h = int((altThumb * dpm) - 15)
                    thumb.paste(img_inf, (inf_w, inf_h))
                    img_inf = None


                    # inserir o thumb na página
                    if cntArqCol > 0:
                        posX += int(largThumb * dpm)

                    if (largPag / largThumb) < cntArqCol + 1:
                        posX = int(config['MargemMidia'] * dpm)
                        posY += int(altThumb * dpm)
                        cntArqCol = 0

                    midia.paste(thumb, (posX, posY))

                    img = None
                    thumb = None

                    cntArqCol+=1
                    cntArqPag+=1
                    cntArq+=1
                

                # salva o arquivo
                filename = str(item['PedidoId']) + "_" + str(item['Id']) + "_" + str(alb) + "_z" + str(cntArq).zfill(5) + ".jpg"
                nomeArquivo = "z" + str(cntPag).zfill(3) + ".jpg"

                midia.save(config['PathFotos'] + str(item['PedidoId']) + "\\" + str(item['Id']) + "\\" + filename, 'JPEG', dpi=(300, 300))
                midia = None

                # Add on upload
                sql  = 'INSERT INTO public."PedidoItemUpload"("PedidoItemId","Path","NomeArquivo","Album","Largura","Altura","Rotacionar","Panoramica","DataCadstro") '
                sql += 'VALUES('+str(item['Id'])+',\''+filename+'\',\''+nomeArquivo+'\',\''+str(alb)+'\','+str(produto['LarguraMidia']*dpm)+','+str(produto['AlturaMidia']*dpm)+',false,false,now())'
                cursor.execute(sql)
                connection.commit()
                
                cntPag+=1
    except:
        None


def PreparaImagensUpload(item):
    global connection
    global cursor

    try:
        cursor.execute('SELECT * FROM public."Produto" WHERE "Id" = '+str(item['ProdutoId']))
        produto = cursor.fetchone()
        
        cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id']))
        pedidoItemUploads = cursor.fetchall()

        #proporção do produto
        proporcao = produto['Largura'] / produto['Altura']

        fotoProporcao = []
        fotoNaoProporcao = []

        #agrupar as fotos por tamanhos, conforme a proporção, e considerar o maior grupo

        agrupado = []
        for foto in pedidoItemUploads:
            fotoProp = foto['Largura'] / foto['Altura']
            fotoPropMin = fotoProp - fotoProp * 0.1
            fotoPropMax = fotoProp * 1.1

            if fotoPropMin <= proporcao and fotoPropMax >= proporcao:
                fotoProporcao.append(foto)

                encontrou = False
                i = 0
                while i < len(agrupado):
                    if agrupado[i].Largura == foto['Largura'] and agrupado[i].Altura == foto['Altura']:
                        agrupado[i].Count+=1
                        encontrou = True

                    i+=1

                if not encontrou:
                    agrupado.append(Grupo(1,foto['Largura'],foto['Altura']))
                
            
            else:
                fotoNaoProporcao.append(foto)
        

        dimensoesPadrao = Grupo(0,None,None)
        x = 0
        while x < len(agrupado):
            if agrupado[x].Count > dimensoesPadrao.Count:
                dimensoesPadrao = agrupado[x]
            x+=1


        
        for imagem in fotoNaoProporcao:
            # se as alturas forem iguais e a largura da foto for 2x a largura do padrão, então é panoramica
            altETdim = imagem['Altura'] == dimensoesPadrao.Altura
            largFGTdim = imagem['Largura'] / 2 >= dimensoesPadrao.Largura - (dimensoesPadrao.Largura * 0.1)
            largFLTdim = imagem['Largura'] / 2 <= dimensoesPadrao.Largura * 1.1

            if altETdim and largFGTdim and largFLTdim:
                sql = 'UPDATE public."PedidoItemUpload" SET "Panoramica" = true WHERE "Id" = ' + str(imagem['Id'])
                cursor.execute(sql)
                connection.commit()
            else:
                # Para as demais, se a largura e altura invertidas da foto for igual a largura e altura padrão, então dar o tombo
                if (imagem['Largura'] / imagem['Altura'] < 1 and proporcao > 1) or (imagem['Largura'] / imagem['Altura'] > 1 and proporcao < 1):
                    sql = 'UPDATE public."PedidoItemUpload" SET "Rotacionar" = true WHERE "Id" = ' + str(imagem['Id'])
                    cursor.execute(sql)
                    connection.commit()

    except:
        None


def ProcessaImagensUpload(item):
    global connection
    global cursor
    global config
    global dpm
    global font

    try:
        cursor.execute('SELECT * FROM public."Produto" WHERE "Id" = '+str(item['ProdutoId']))
        produto = cursor.fetchone()

        cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id']))
        pedidoItemUploads = cursor.fetchall()


        albuns = []
        for arq in pedidoItemUploads:
            if not arq['Album'] in albuns:
                albuns.append(arq['Album'])
        
        for alb in albuns:
            cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id'])+' AND "Album" = \''+str(alb)+'\' ORDER BY "NomeArquivo"')
            fotos = cursor.fetchall()

            cntArq = 1
            for foto in fotos:
                fileImage = config['PathFotos'] + str(item['PedidoId']) + "/" + str(item['Id']) + "/" + foto['Path']
                
                img = Image.open(fileImage)
                icc_profile = img.info.get('icc_profile')
                
                if foto['Panoramica']:
                    strct = foto['NomeArquivo'].split('.')
                    ext = strct[-1]
                    del strct[-1]

                    filenameA = str(item['PedidoId']) + "_" + str(item['Id']) + "_" + str(alb) + "_" + str(cntArq).zfill(5) + ".jpg"
                    cntArq+=1
                    filenameB = str(item['PedidoId']) + "_" + str(item['Id']) + "_" + str(alb) + "_" + str(cntArq).zfill(5) + ".jpg"

                    nomeArquivoA = '.'.join(strct) + "a." + ext
                    nomeArquivoB = '.'.join(strct) + "b." + ext

                    imgA = img.crop((0,0, img.size[0]/2, img.size[1]))
                    imgB = img.crop(((img.size[0]/2)+1, 0, img.size[0], img.size[1]))

                    SalvarImagem(produto, imgA, icc_profile, filenameA)
                    SalvarImagem(produto, imgB, icc_profile, filenameB)

                    sql  = 'INSERT INTO public."PedidoItemArquivo"("PedidoItemId","Sequencia","Album","Path","NomeArquivo","DataUpload") VALUES'
                    sql += '('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filenameA+'\',\''+nomeArquivoA+'\', now()), '
                    sql += '('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filenameB+'\',\''+nomeArquivoB+'\', now()) '
                    cursor.execute(sql)
                    connection.commit()

                    imgA = None
                    imgB = None

                else:
                    filename = str(item['PedidoId']) + "_" + str(item['Id']) + "_" + str(alb) + "_" + str(cntArq).zfill(5) + ".jpg"
                    
                    SalvarImagem(produto, img, icc_profile, filename)
                    sql  = 'INSERT INTO public."PedidoItemArquivo"("PedidoItemId","Sequencia","Album","Path","NomeArquivo","DataUpload") '
                    sql += 'VALUES('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filename+'\',\''+foto['NomeArquivo']+'\', now()) '
                    cursor.execute(sql)
                    connection.commit()

                os.remove(fileImage)

                sql = 'DELETE FROM public."PedidoItemUpload" WHERE "Id" = '+str(foto['Id'])
                cursor.execute(sql)
                connection.commit()

                img = None

                cntArq+=1
    except:
        None


def Importacao():
    global connection
    global cursor

    try:
        sql = 'SELECT * FROM public."PedidoItem" WHERE "DataUpload" IS NOT NULL AND "DataProcessamentoUpload" IS NULL'
        cursor.execute(sql)
        itens = cursor.fetchall()

        for item in itens:
            if item['Index'] and item['DataProcessamentoIndex'] == None:
                Index(item)
                sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoIndex" = now() WHERE "Id" = '+str(item['Id'])
                cursor.execute(sql)
                connection.commit()
            
            if item['DataProcessamentoPreparacao'] == None:
                PreparaImagensUpload(item)

                sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoPreparacao" = now() WHERE "Id" = '+str(item['Id'])
                cursor.execute(sql)
                connection.commit()

            if item['DataProcessamentoUpload'] == None:
                ProcessaImagensUpload(item)

                sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoUpload" = now() WHERE "Id" = '+str(item['Id'])
                cursor.execute(sql)
                connection.commit()

            if item['DataAprovacao'] == None and config['AprovarAutomaticamente']:
                sql = 'UPDATE public."PedidoItem" SET "DataAprovacao" = now() WHERE "Id" = '+str(item['Id'])
                cursor.execute(sql)
                connection.commit()
            
    except:
        None


##### IMPOSIÇÃO #####

def MakeDirectory(folder):
    directory = ''
    #pastas = folder.replace('\\\\','/').split('/')
    pastas = folder.split('/')
    for pasta in pastas:
        directory = os.path.join(directory, pasta) # Windows
        #directory += '/'+pasta # Linux
        if not os.path.exists(directory):
            os.mkdir(directory)

def MargemCorte(img, supDir, infDir, supEsq, infEsq):
    draw = ImageDraw.Draw(img)
    
    try: draw.line([(supDir[0]-(5*dpm), supDir[1]), (supDir[0]-(2*dpm), supDir[1])], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0], supDir[1]-(5*dpm)), (supDir[0], supDir[1]-(2*dpm))], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0]+(2*dpm), supDir[1]), (supDir[0]+(5*dpm), supDir[1])], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0], supDir[1]+(2*dpm)), (supDir[0], supDir[1]+(5*dpm))], fill="black", width=0) 
    except: print('supDir', supDir)

    try: draw.line([(infDir[0]-(5*dpm), infDir[1]), (infDir[0]-(2*dpm), infDir[1])], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0], infDir[1]-(5*dpm)), (infDir[0], infDir[1]-(2*dpm))], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0]+(2*dpm), infDir[1]), (infDir[0]+(5*dpm), infDir[1])], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0], infDir[1]+(2*dpm)), (infDir[0], infDir[1]+(5*dpm))], fill="black", width=0) 
    except: print('infDir', infDir)

    try: draw.line([(supEsq[0]-(5*dpm), supEsq[1]), (supEsq[0]-(2*dpm), supEsq[1])], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0], supEsq[1]-(5*dpm)), (supEsq[0], supEsq[1]-(2*dpm))], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0]+(2*dpm), supEsq[1]), (supEsq[0]+(5*dpm), supEsq[1])], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0], supEsq[1]+(2*dpm)), (supEsq[0], supEsq[1]+(5*dpm))], fill="black", width=0) 
    except: print('supEsq', supEsq)

    try: draw.line([(infEsq[0]-(5*dpm), infEsq[1]), (infEsq[0]-(2*dpm), infEsq[1])], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0], infEsq[1]-(5*dpm)), (infEsq[0], infEsq[1]-(2*dpm))], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0]+(2*dpm), infEsq[1]), (infEsq[0]+(5*dpm), infEsq[1])], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0], infEsq[1]+(2*dpm)), (infEsq[0], infEsq[1]+(5*dpm))], fill="black", width=0) 
    except: print('infEsq', infEsq)
    
    return img

def AddInfo(midea, inf):
    global path_root
    global font_path
    global barras_tmp

    try:
        if inf['Tipo'] == 'texto':
            draw = ImageDraw.Draw(midea)
            #cria imagem de texto
            textsize = draw.textbbox((0,0),inf['Texto'], font)
            txt_width = textsize[2] - textsize[0] + 10
            txt_height = textsize[3] - textsize[1] + 10
            # txt_width = int(draw.textlength(inf['Texto'], font, 'ltr'))
            # txt_height = int(draw.textlength(inf['Texto'], font, 'ttb'))
            
            #txt_width, txt_height = draw.textsize(inf['Texto'], font)
            img_inf = Image.new('RGB', (txt_width, txt_height), (255, 255, 255))
            draw = ImageDraw.Draw(img_inf)
            draw.text((5, 0), inf['Texto'], (0,0,0), font=font)

        elif inf['Tipo'] == 'barras':
            options = {
                'module_width':0.4,
                'module_height':8,
                #'text_distance':1,
                'text_distance':2,
                'font_path':font_path,
                #'font_size':10
                'font_size':4
            }

            filename = barras_tmp + '/'+str(inf['Texto'])
            codeClass = barcode.get_barcode_class('code128')
            codigo_barra = codeClass(str(inf['Texto']).zfill(3), writer=ImageWriter())
            
            if not os.path.exists(filename+'.png'):
                filename = codigo_barra.save(filename, options = options)
            else: 
                filename += '.png'
            
            img_inf = Image.open(filename)



        # # rotaciona imagem de texto
        # if int(inf['Orientacao']) == 90:
        #     img_inf = img_inf.transpose(Image.Transpose.ROTATE_90)
        # elif int(inf['Orientacao']) == 180:
        #     img_inf = img_inf.transpose(Image.Transpose.ROTATE_180)
        # elif int(inf['Orientacao']) == 270:
        #     img_inf = img_inf.transpose(Image.Transpose.ROTATE_270)
        if int(inf['Orientacao']) == 90:
            img_inf = img_inf.transpose(Image.ROTATE_90)
        elif int(inf['Orientacao']) == 180:
            img_inf = img_inf.transpose(Image.ROTATE_180)
        elif int(inf['Orientacao']) == 270:
            img_inf = img_inf.transpose(Image.ROTATE_270)

        # salva texto na imagem
        inf_w = int((inf['PosX']*dpm)-(img_inf.size[0]/2))
        inf_h = int((inf['PosY']*dpm)-(img_inf.size[1]/2))
        midea.paste(img_inf, (inf_w, inf_h))
        #midea.paste(img_inf, (int(inf['PosX']*dpm), int(inf['PosY']*dpm)))
        img_inf = None

    except Exception as e: 
        if debug:
            print(e)
    finally:
        return midea


def DuplexFV(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp
    
    try:
        img = Image.open(item)
        midea = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

        if cnt % 2 == 0:
            # frente
            (larg, alt, pag) = (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm), 0)
        else:
            # verso
            (larg, alt, pag) = (int(produto['DeslocamentoVersoX']*dpm), int(produto['DeslocamentoVersoY']*dpm), 1)

        midea.paste(img, (larg, alt))
        
        supDir = (larg, alt)
        infDir = (larg, alt+img.size[1])
        supEsq = (larg+img.size[0], alt)
        infEsq = (larg+img.size[0], alt+img.size[1])
        
        midea = MargemCorte(midea, supDir, infDir, supEsq, infEsq)

        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)
        
        for inf in infos:
            if inf['Pagina'] == pag:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': inf['Tipo'],
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea = AddInfo(midea, i)

        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]
        midea.save(path_tmp + '/' + filename, 'JPEG', dpi=(300, 300))
        
    except Exception as e: 
        if debug:
            print(e)
    
    finally:
        img = None
        midea = None

def DuplexPV(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp

    try:
        img = Image.open(item)
        midea_f = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))
        midea_v = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

        midea_f.paste(img, (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm)))

        supDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm))
        infDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        supEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm))
        infEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        
        midea_f = MargemCorte(midea_f, supDir, infDir, supEsq, infEsq)
        
        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)
        
        for inf in infos:
            if inf['Pagina'] == 0:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea_f = AddInfo(midea_f, i)
            elif inf['Pagina'] == 1:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto'].replace("{nome_arquivo}", nome_arquivo).replace("{seq_foto}", str(cnt+1)),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea_v = AddInfo(midea_v, i)

        
        

        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]
        fnst = filename.split('.')
        filename_f = str(fnst[0])+'_f.'+fnst[1]
        filename_v = str(fnst[0])+'_v.'+fnst[1]
        
        midea_f.save(path_tmp + '/' + filename_f, 'JPEG', dpi=(300, 300))
        midea_v.save(path_tmp + '/' + filename_v, 'JPEG', dpi=(300, 300))

    except Exception as e: 
        if debug:
            print(e)
            
        if os.path.exists(path_tmp + '/' + filename_f):
            os.remove(path_tmp + '/' + filename_f)
        if os.path.exists(path_tmp + '/' + filename_v):
            os.remove(path_tmp + '/' + filename_v)

    finally:
        img = None
        midea_f = None
        midea_v = None

def Simplex(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp
    
    try:
        img = Image.open(item)
        midea = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))
        
        midea.paste(img, (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm)))
        
        supDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm))
        infDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        supEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm))
        infEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        
        midea = MargemCorte(midea, supDir, infDir, supEsq, infEsq)

        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)

        for inf in infos:
            if inf['Pagina'] == 0:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                }
                
                midea = AddInfo(midea, i)

        
        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]
        midea.save(path_tmp + '/' + filename, 'JPEG', dpi=(300, 300))
        
        #return_dict[len(return_dict)] = path_tmp + '/' + filename
    except Exception as e: 
        if debug:
            print(e)

    finally:
        img = None
        midea = None

def Imposicao():
    global connection
    global cursor
    
    barras_tmp = os.path.join("C:\\temp","barras")
    MakeDirectory(barras_tmp)

    sql = 'SELECT * FROM public."Config"'
    cursor.execute(sql)
    config = cursor.fetchone()
    # config['PathFotos'] = "/home/ayres/Documentos/Projetos/Piovelli/impressao_pro/Storage/fotos/"
    # config['PathPdf'] = "/home/ayres/Documentos/Projetos/Piovelli/impressao_pro/Storage/PDF/"


    sql = 'SELECT * FROM public."PedidoItem" WHERE "DataAprovacao" IS NOT NULL AND "DataImposicao" IS NULL'
    cursor.execute(sql)
    result = cursor.fetchall()

    for row in result:
        try:
            sql = 'SELECT * FROM public."Produto" WHERE "Id" = '+str(row['ProdutoId'])
            cursor.execute(sql)
            produto = cursor.fetchone()

            sql = 'SELECT * FROM public."ProdutoInformacao" WHERE "ProdutoId" = '+str(produto['Id'])
            cursor.execute(sql)
            informacoes = cursor.fetchall()

            
            sql = 'SELECT "Album" FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' GROUP BY "Album" ORDER BY "Album"'
            cursor.execute(sql)
            albuns = cursor.fetchall()
            
            return_dict = []
            for album in albuns:
                nome_arqivo = str(row['PedidoId'])+'_'+str(row['Id'])+'_'+album['Album']

                sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\''
                cursor.execute(sql)
                alb = cursor.fetchone()
                if alb == None:
                    sql = 'INSERT INTO public."PedidoItemArquivoAlbum"("PedidoItemId", "Album", "NomeArquivo", "DataCadastro", "DataProcessamento") '
                    sql += 'VALUES ('+str(row['Id'])+', '+str(album['Album'])+', \''+nome_arqivo+'\', now(), now());'
                    cursor.execute(sql)
                    connection.commit()

                    sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\''
                    cursor.execute(sql)
                    alb = cursor.fetchone()

                #path_tmp = "C:/temp/"+str(row['Id'])+"/"+str(album['Album'])
                path_tmp = os.path.join("C:\\temp",str(row['Id']))
                if os.path.exists(path_tmp):
                    shutil.rmtree(path_tmp)
                
                MakeDirectory(path_tmp)
                
                sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\''
                cursor.execute(sql)
                arquivos = cursor.fetchall()
                
                if len(arquivos) > 0:
                    cnt = 0
                    for arq in arquivos:

                        infos = []
                        for inf in informacoes:
                            infos.append(inf)

                        path = config['PathFotos']+str(row['PedidoId'])+'/'+str(row['Id'])+'/'+arq['Path']
                        if produto['DisposicaoImpressao'] == 'D':
                            if produto['DisposicaoImagem'] == 'D':
                                DuplexFV(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)
                            else:
                                DuplexPV(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)
                        else:
                            Simplex(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)

                        cnt+=1
                    
                    lista = os.listdir(path_tmp)
                    lista_saida = []
                    for l in lista:
                        lista_saida.append(os.path.join(path_tmp, l))
                    
                    lista_saida.sort()

                    if len(lista_saida) > 0:
                        pdfFile = config['PathPdf']+nome_arqivo+'.pdf'
                        with open(pdfFile, "wb") as f:
                            f.write(img2pdf.convert(lista_saida))

                    os.chmod(pdfFile, 0o777)

                shutil.rmtree(path_tmp)

            sql = 'UPDATE public."PedidoItem" SET "DataImposicao" = now() WHERE "Id" = '+str(row["Id"])
            cursor.execute(sql)
            connection.commit()
        except:
            None



def getMachine_addr():
	os_type = sys.platform.lower()
	if "win" in os_type:
		command = "wmic bios get serialnumber"
	elif "linux" in os_type:
		command = "hal-get-property --udi /org/freedesktop/Hal/devices/computer --key system.hardware.uuid"
	elif "darwin" in os_type:
		command = "ioreg -l | grep IOPlatformSerialNumber"
	return os.popen(command).read().replace("\n","").replace("	","").replace(" ","")


'''
class AppServerSvc (win32serviceutil.ServiceFramework):
    _svc_name_ = "ImpressaoService"
    _svc_display_name_ = "Impressão - Service"

    def __init__(self,args):
        win32serviceutil.ServiceFramework.__init__(self,args)
        self.hWaitStop = win32event.CreateEvent(None,0,0,None)
        socket.setdefaulttimeout(60)

    def SvcStop(self):
        self.ReportServiceStatus(win32service.SERVICE_STOP_PENDING)
        win32event.SetEvent(self.hWaitStop)

    def SvcDoRun(self):
        servicemanager.LogMsg(servicemanager.EVENTLOG_INFORMATION_TYPE,
                              servicemanager.PYS_SERVICE_STARTED,
                              (self._svc_name_,''))
        self.main()

    def main(self):
        Importacao()
        Imposicao()

        print('getMachine_addr', getMachine_addr())

        import subprocess
        code = subprocess.check_output('wmic bios get serialnumber').decode("utf-8") 
        print('subprocess', code)
        
        code2 = subprocess.Popen('dmidecode.exe -s system-uuid'.split())
        print('dmidecode.exe', code2)

        import uuid    
        un = uuid.UUID(int=uuid.getnode())
        print('uuid', un)

        pass
'''

if __name__ == '__main__':
    print('getMachine_addr', getMachine_addr())

    import subprocess
    code = subprocess.check_output('wmic bios get serialnumber').decode("utf-8") 
    print('subprocess', code)
    
    code2 = subprocess.Popen('dmidecode.exe -s system-uuid'.split())
    print('dmidecode.exe', code2)

    import uuid    
    un = uuid.UUID(int=uuid.getnode())
    print('uuid', un)

    #win32serviceutil.HandleCommandLine(AppServerSvc)