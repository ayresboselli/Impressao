import psycopg2.extras
import datetime
import os

from PIL import Image, ImageDraw, ImageFont
from dotenv import load_dotenv
from multiprocessing import Process, Manager

debug = False
path_root = os.path.dirname(os.path.realpath(__file__))

dotenv_path = path_root+'/.env'
load_dotenv(dotenv_path)

vCPU = os.cpu_count()

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

font_path = 'C:/Piovelli/sans-serif.ttf'
font = ImageFont.truetype(font_path, int(config['TamanhoFonte']))


class Grupo:
    def __init__(self,Count,Largura,Altura):
        self.Count = Count
        self.Largura = Largura
        self.Altura = Altura


def SalvarImagem(item, produto, img, icc_profile, album, filename):
    global config
    global dpm

    retorno = True

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

        img.save(config['PathFotos'] + str(item['PedidoId']) + "\\" + str(item['Id']) + "\\" + str(album) + "\\" + filename, 'JPEG', dpi=(config['DPI'], config['DPI']), icc_profile=icc_profile)

        thumbLarg = 600 * 100
        thumbAlt = int(thumbLarg / img.size[0] * img.size[1])
        img.thumbnail((thumbLarg, thumbAlt))
        
        img.save(config['WwwPath'] + "/img/thumbs/" + filename, 'JPEG')
            
    except:
        retorno = False
    finally:
        img = None
        return retorno
    

def IndexMP(item, produto, alb, processaIndex):
    global config

    try:
        # arquivos por página
        largPag = float(produto['LarguraMidia']) - float(config['MargemMidia'])
        altPag = float(produto['AlturaMidia']) - float(config['MargemMidia'])
        largThumb = float(config['LarguraThumb']) + float(config['MargemThumb']) * 2
        altThumb = float(config['AlturaThumb']) + (float(config['MargemThumb']) * 2)
        nCol = int((largPag / largThumb))
        nRow = int(((altPag * dpm) / ((altThumb * dpm) + float(config['TamanhoFonte']))))
        
        margemX = int((largPag - ((largPag // largThumb) * largThumb)) / 2 * dpm)
        margemY = int((altPag - ((altPag // altThumb) * altThumb)) / 2 * dpm)

        # arquivos por album
        cursor.execute('SELECT * FROM public."PedidoItemUpload" WHERE "PedidoItemId" = '+str(item['Id'])+' AND "Album" = \''+str(alb)+'\' ORDER BY "NomeArquivo"')
        arqList = cursor.fetchall()

        cntArq = 0
        cntPag = 0
        while cntArq < len(arqList):
            # cria a página
            midia = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

            
            posY = margemY + int(config['MargemMidia'])
            for y in range(nRow):
                posX = margemX + int(config['MargemMidia'] * dpm)
                if y > 0:
                    posY += int(altThumb * dpm)

                for x in range(nCol):    
                    # criar o thumb
                    if cntArq < len(arqList):
                        img = Image.open(config['PathFotos'] + str(item['PedidoId']) + "/" + str(item['Id']) + "/" + str(alb) + "/" + arqList[cntArq]['Path'])
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
                        inf_h = int((altThumb * dpm) - 25)
                        thumb.paste(img_inf, (inf_w, inf_h))
                        img_inf = None

                        # inserir o thumb na página
                        if x > 0:
                            posX += int(largThumb * dpm)

                        midia.paste(thumb, (posX, posY))

                        img = None
                        thumb = None

                        cntArq+=1


            # salva o arquivo
            filename = str(item['PedidoId']) + "_" + str(item['Id']) + "_" + str(alb) + "_z" + str(cntArq).zfill(5) + ".jpg"
            nomeArquivo = "z" + str(cntPag).zfill(3) + ".jpg"

            midia.save(config['PathFotos'] + str(item['PedidoId']) + "\\" + str(item['Id']) + "\\" + str(alb) + "\\" + filename, 'JPEG', dpi=(300, 300))
            midia = None
            
            # Add on upload
            sql  = 'INSERT INTO public."PedidoItemUpload"("PedidoItemId","Path","NomeArquivo","Album","Largura","Altura","Rotacionar","Panoramica","DataCadstro") '
            sql += 'VALUES('+str(item['Id'])+',\''+filename+'\',\''+nomeArquivo+'\',\''+str(alb)+'\','+str(produto['LarguraMidia']*dpm)+','+str(produto['AlturaMidia']*dpm)+',false,false,now())'
            cursor.execute(sql)
            connection.commit()
            
            processaIndex[len(processaIndex)] = [alb,True]

            cntPag+=1
    except:
        processaIndex[len(processaIndex)] = [alb,False]
    

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
        
        manager = Manager()
        processaIndex = manager.dict()

        process_list = []
        cnt = 0
        while cnt < len(albuns):
            cnt_p = 0

            while cnt_p <= vCPU:
                if cnt < len(albuns):
                    p = Process(target=IndexMP, args = [item, produto, albuns[cnt], processaIndex])
                    p.start()
                    process_list.append(p)
                    cnt += 1
                    cnt_p += 1
                else:
                    break
            
            for process in process_list:
                process.join()
                
        for i in processaIndex.values():
            if not i[1]:
                return False

        return True
    except:
        return False


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

        return True
    except:
        return False


def ProcessaImagemUploadMP(item, produto, foto, alb, cntArq, processaImaemUpload):
    global connection
    global cursor

    salvo = False
    try:
        fileImage = config['PathFotos'] + str(item['PedidoId']) + "/" + str(item['Id']) + "/" + alb + "/" + foto['Path']
                
        img = Image.open(fileImage)
        icc_profile = img.info.get('icc_profile')
        
        if foto['Panoramica']:
            strct = foto['NomeArquivo'].split('.')
            ext = strct[-1]
            del strct[-1]

            filenameA = str(item['Contrato']) + " " + str(alb) + " " + str(cntArq).zfill(5) + ".jpg"
            cntArq+=1
            filenameB = str(item['Contrato']) + " " + str(alb) + " " + str(cntArq).zfill(5) + ".jpg"

            nomeArquivoA = '.'.join(strct) + "a." + ext
            nomeArquivoB = '.'.join(strct) + "b." + ext

            imgA = img.crop((0,0, img.size[0]/2, img.size[1]))
            imgB = img.crop(((img.size[0]/2)+1, 0, img.size[0], img.size[1]))

            salvoA = SalvarImagem(item, produto, imgA, icc_profile, alb, filenameA)
            salvoB = SalvarImagem(item, produto, imgB, icc_profile, alb, filenameB)

            if salvoA and salvoB:
                salvo = True
                sql  = 'INSERT INTO public."PedidoItemArquivo"("PedidoItemId","Sequencia","Album","Path","NomeArquivo","DataUpload") VALUES'
                sql += '('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filenameA+'\',\''+nomeArquivoA+'\', now()), '
                sql += '('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filenameB+'\',\''+nomeArquivoB+'\', now()) '
                cursor.execute(sql)

            imgA = None
            imgB = None

        else:
            filename = str(item['Contrato']) + " " + str(alb) + " " + str(cntArq).zfill(5) + ".jpg"
            
            salvo = SalvarImagem(item, produto, img, icc_profile, alb, filename)
            
            if salvo:
                sql  = 'INSERT INTO public."PedidoItemArquivo"("PedidoItemId","Sequencia","Album","Path","NomeArquivo","DataUpload") '
                sql += 'VALUES('+str(item['Id'])+',0,\''+str(alb)+'\',\''+filename+'\',\''+foto['NomeArquivo']+'\', now()) '
                cursor.execute(sql)
        
        if salvo:
            os.remove(fileImage)

            sql = 'DELETE FROM public."PedidoItemUpload" WHERE "Id" = '+str(foto['Id'])
            cursor.execute(sql)
            connection.commit()

        img = None
        
        connection.commit()
    except:
        connection.rollback()
    finally:
        processaImaemUpload[cntArq] = [foto['Path'], salvo]


def ProcessaImagensUpload(item):
    global connection
    global cursor
    global config
    global dpm
    global font

    retorno = True

    try:
        if item['Contrato'] == None:
            item['Contrato'] = item['Id']

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


            manager = Manager()
            processaImaemUpload = manager.dict()
            process_list = []
            cnt = 0
            while cnt < len(fotos):
                cnt_p = 0

                while cnt_p <= vCPU:
                    if cnt < len(fotos):

                        p = Process(target=ProcessaImagemUploadMP, args = [item, produto, fotos[cnt], alb, cnt, processaImaemUpload])
                        p.start()
                        process_list.append(p)
                        cnt += 1
                        cnt_p += 1
                    else:
                        break
                
                for process in process_list:
                    process.join()

            for piu in processaImaemUpload.values():
                if not piu[1]:
                    retorno = False

        
        return retorno
    except:
        return False


def Start():
    global connection
    global cursor

    try:
        sql = 'SELECT "I".*, "P"."Contrato" '
        sql += 'FROM public."PedidoItem" AS "I" '
        sql += 'JOIN public."Pedido" AS "P" ON "P"."Id" = "I"."PedidoId" '
        sql += 'WHERE "I"."DataUpload" IS NOT NULL AND "I"."DataAprovacao" IS NULL'

        cursor.execute(sql)
        itens = cursor.fetchall()
        
        for item in itens:
            if item['Index'] and item['DataProcessamentoIndex'] == None:
                if Index(item):
                    sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoIndex" = now() WHERE "Id" = '+str(item['Id'])
                    cursor.execute(sql)
                    connection.commit()

                    item['DataProcessamentoIndex'] = datetime.datetime.now()
            
            if (not item['Index'] or (item['Index'] and item['DataProcessamentoIndex'] != None)) and item['DataProcessamentoPreparacao'] == None:
                if PreparaImagensUpload(item):
                    sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoPreparacao" = now() WHERE "Id" = '+str(item['Id'])
                    cursor.execute(sql)
                    connection.commit()

                    item['DataProcessamentoPreparacao'] = datetime.datetime.now()
            
            if item['DataProcessamentoPreparacao'] != None and item['DataProcessamentoUpload'] == None:
                if ProcessaImagensUpload(item):
                    sql = 'UPDATE public."PedidoItem" SET "DataProcessamentoUpload" = now() WHERE "Id" = '+str(item['Id'])
                    cursor.execute(sql)
                    connection.commit()

                    item['DataProcessamentoUpload'] = datetime.datetime.now()
            
            if (not item['Index'] or (item['Index'] and item['DataProcessamentoIndex'] != None)) and item['DataProcessamentoPreparacao'] != None and item['DataProcessamentoUpload'] != None:
                if item['DataAprovacao'] == None and config['AprovarAutomaticamente']:
                    sql = 'UPDATE public."PedidoItem" SET "DataAprovacao" = now() WHERE "Id" = '+str(item['Id'])
                    cursor.execute(sql)
                    connection.commit()
            
    except:
        None

